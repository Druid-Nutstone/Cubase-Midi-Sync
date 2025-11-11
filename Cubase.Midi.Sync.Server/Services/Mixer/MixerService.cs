using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.Server.Services.Windows;

namespace Cubase.Midi.Sync.Server.Services.Mixer
{
    public class MixerService : IMixerService
    {
        private readonly ICacheService cacheService;    

        private readonly IMidiService midiService;
        
        private readonly IServiceProvider services;  
        
        private readonly ILogger<MixerService> logger;

        private readonly ICubaseWindowMonitor cubaseWindowMonitor;

        private ICategoryService categoryService;

        private CubaseActiveWindowCollection windowsCollection;

        private CubaseMidiCommandCollection cubaseMidiCommands;

        public MixerService(ILogger<MixerService> logger,
                            ICacheService cacheService, 
                            IMidiService midiService, 
                            ICubaseWindowMonitor cubaseWindowMonitor,
                            IServiceProvider services)
        {
            this.cacheService = cacheService;    
            this.midiService = midiService;
            this.cubaseWindowMonitor = cubaseWindowMonitor;
            this.logger = logger;   
            this.cubaseMidiCommands = new CubaseMidiCommandCollection(CubaseServerConstants.KeyCommandsFileLocation);
            this.services = services;
            this.categoryService = this.services.GetKeyedService<ICategoryService>(CubaseServiceConstants.KeyService);
            this.cubaseWindowMonitor.RegisterForWindowEvents(this.CubaseWindowsEvent);
        }

        private void CubaseWindowsEvent(CubaseActiveWindowCollection cubaseActiveWindows)
        {
            this.windowsCollection = cubaseActiveWindows;
        }
        
        public async Task<CubaseMixerCollection> GetMixer()
        {
            this.logger.LogInformation($"MixerService - Loading CubaseMixer Collection. The current Success flag is {this.cacheService.CubaseMixer.Success} with an error message of {this.cacheService.CubaseMixer.ErrorMessage}");
            await Task.Delay(1);
            return this.cacheService.CubaseMixer;
        }
        
        public async Task<CubaseMixerCollection> MixerCommand(CubaseMixer cubaseMixer)
        {
            if (!string.IsNullOrEmpty(cubaseMixer.KeyAction))
            {
                this.logger.LogInformation($"Executing mixer Key command {cubaseMixer.KeyAction}");
                this.categoryService.ProcessAction(ActionEvent.Create(CubaseAreaTypes.Midi, cubaseMixer.KeyAction));
            }
            else
            {
                var commands = cacheService.CubaseMixer.GetCommands(cubaseMixer.Command);
                foreach (var cmd in commands)
                {
                    await this.SendMidiCommand(cmd);
                }
            }
            return cacheService.CubaseMixer.WithMixerNames(this.cubaseWindowMonitor.MixerConsoles);
        }

        public async Task<CubaseMixerResponse> MixerRequest(CubaseMixerRequest cubaseMixerRequest)
        {
            switch (cubaseMixerRequest.Command)
            {
                case CubaseMixerCommand.MixerStaticCommand:
                    var actionEvent = cubaseMixerRequest.GetData<ActionEvent>();
                    var knownCommand = Enum.Parse<KnownCubaseMidiCommands>(actionEvent.Action);
                    var commandList = this.cacheService.CubaseMixer.GetCommands(knownCommand);
                    var targetWindow = cubaseMixerRequest.TargetMixer ?? this.cubaseWindowMonitor.MixerConsoles.First();
                    this.FocusWindow(targetWindow);
                    if (!string.IsNullOrEmpty(cubaseMixerRequest.TargetMixer))
                    {
                        this.cubaseWindowMonitor.CubaseWindows
                                                .GetWindowByName(cubaseMixerRequest.TargetMixer)
                                                .Focus();
                    }
                    foreach (var cmd in commandList)
                    {
                        await this.SendMidiCommand(cmd);
                    }
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerStaticCommand);
                case CubaseMixerCommand.OpenMixer:
                    if (string.IsNullOrEmpty(cubaseMixerRequest.TargetMixer))
                    {
                        await this.SendMidiCommand(this.cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Mixer)); 
                        if (await WaitForMixConsoleCount(1))
                        {
                            this.cubaseWindowMonitor.CubaseWindows
                                                    .GetWindowByName(this.cubaseWindowMonitor.MixerConsoles.First())
                                                    .Maximise()
                                                    .Focus();
                        }
                        else
                        {
                            return CubaseMixerResponse.CreateError("Timed out waiting for Cubase Mixer to open.");
                        }
                    }
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerCollection, await GetMixer());
                case CubaseMixerCommand.MixerCollection:
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerCollection, await GetMixer());
                default:
                    return CubaseMixerResponse.CreateError("Unknown Mixer command.");
            }
        }

        private void FocusWindow(string windowName)
        {
            var window = this.cubaseWindowMonitor.CubaseWindows.GetWindowByName(windowName);
            if (window != null)
            {
                window.Focus();
            }
        }

        private async Task<bool> WaitForMixConsoleCount(int consoleCount)
        {
            var maxCount = 10000;
            var currentCount = 0;
            while (this.windowsCollection.CountOfMixers() < consoleCount)
            {
                currentCount++;
                await Task.Run(() => Task.Delay(50));
                if (currentCount >= maxCount)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task SendMidiCommand(CubaseMidiCommand cmd)
        {
            this.midiService.SendMidiMessage(cmd);
            await Task.Delay(20); // Small delay to ensure Cubase processes the command
        }
    }
}
