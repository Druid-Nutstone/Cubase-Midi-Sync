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
using Cubase.Midi.Sync.WindowManager.Services.Win;
using static Cubase.Midi.Sync.WindowManager.Services.Win.WindowManagerService;

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

        private CubaseActiveWindowCollection windowsCollection = new CubaseActiveWindowCollection();

        private CubaseMidiCommandCollection cubaseMidiCommands;

        private bool mixersEnabled = false;

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
            if (mixersEnabled == false)
            {
                return;
            }
            if (this.windowsCollection.HaveAnyMixers())
            {
                if (this.cubaseWindowMonitor.GetMixerWindows().Count == 1)
                {
                    var firstMixer = this.cubaseWindowMonitor.GetMixerWindows().FirstOrDefault();
                    if (firstMixer != null)
                    {
                        if (firstMixer.State != WindowManager.Models.WindowState.Maximized)
                        {
                            
                            firstMixer.Maximise().Focus();
                        }
                    }
                }
                else
                {
                    if (this.windowsCollection.CountOfMixers() > 1)
                    {
                        BuildMixerLayout();
                    }
                    else
                    {
                        if (this.windowsCollection.CountOfMixers() == 0)
                        {
                            // No mixers
                            var mainCubaseWindow = this.windowsCollection.GetPrimaryWindow();
                            if (mainCubaseWindow != null)
                            {
                                this.cubaseWindowMonitor.CubaseWindows.GetWindowByName(mainCubaseWindow.Name)
                                       .Maximise()
                                       .Focus();
                            }
                        }
                    }
                }
            }
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
            this.mixersEnabled = true;
            switch (cubaseMixerRequest.Command)
            {
                case CubaseMixerCommand.FocusMixer:
                    this.FocusWindow(cubaseMixerRequest.TargetMixer ?? this.cubaseWindowMonitor.MixerConsoles.First());
                    return CubaseMixerResponse.Create(CubaseMixerCommand.FocusMixer);
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
                    await this.AddOrFocusMixer(cubaseMixerRequest.TargetMixer ?? string.Empty);
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerCollection, await GetMixer());
                case CubaseMixerCommand.RestoreMixers:
                    BuildMixerLayout();
                    return CubaseMixerResponse.Create(CubaseMixerCommand.RestoreMixers);
                case CubaseMixerCommand.MixerCollection:
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerCollection, await GetMixer());
                case CubaseMixerCommand.CloseMixers:
                    this.mixersEnabled = false;
                    var mixerWindows = this.cubaseWindowMonitor.GetMixerWindows();
                    foreach (var mixer in mixerWindows)
                    {
                        mixer.Close();
                    }
                    return CubaseMixerResponse.Create(CubaseMixerCommand.CloseMixers);
                case CubaseMixerCommand.ProjectWindow:
                    this.mixersEnabled = false;
                    var mainCubaseWindow = this.cubaseWindowMonitor.CubaseWindows.GetPrimaryWindow();

                    if (mainCubaseWindow != null)
                    {
                        this.cubaseWindowMonitor.CubaseWindows
                                       .GetActiveWindows()
                                       .Where(w => w.Name.StartsWith("MixConsole"))
                                       .ToList()
                                       .ForEach(w => w.Minimise());

                        mainCubaseWindow.Maximise()
                                        .Focus();
                    }
                    return CubaseMixerResponse.Create(CubaseMixerCommand.ProjectWindow);
                default:
                    return CubaseMixerResponse.CreateError("Unknown Mixer command.");
            }
        }

        private async Task AddOrFocusMixer(string mixerName)
        {
            if (this.cubaseWindowMonitor.MixerExists(mixerName))
            {
                this.FocusWindow(mixerName);
                return;
            }
            else
            {
                var currentMixerCount = cubaseWindowMonitor.MixerConsoles.Count == 0 ? 1 : cubaseWindowMonitor.MixerConsoles.Count;
                var midiCommand = KnownCubaseMidiCommands.Mixer;
                var windowBits = mixerName.Split(' ');
                if (windowBits.Length >= 2 && int.TryParse(windowBits[1], out int mixerNumber))
                {
                    switch (mixerNumber)
                    {
                        case 2:
                            midiCommand = KnownCubaseMidiCommands.Mixer_2;
                            break;
                        case 3:
                            midiCommand = KnownCubaseMidiCommands.Mixer_3;
                            break;
                        case 4:
                            midiCommand = KnownCubaseMidiCommands.Mixer_4;
                            break;
                    }
                    currentMixerCount++;
                }
                await this.SendMidiCommand(this.cubaseMidiCommands.GetMidiCommandByName(midiCommand));
                await WaitForMixConsoleCount(currentMixerCount);
                if (currentMixerCount > 1)
                {
                    BuildMixerLayout();
                    // this.FocusWindow(mixerName);
                }
                else
                {
                    var mixerWindow = this.cubaseWindowMonitor.CubaseWindows.GetWindowThatStartsWith(mixerName);
                    if (mixerWindow != null)
                    {
                        mixerWindow.Maximise()
                                   .Focus();
                    }
                }
            }
        }

        private void BuildMixerLayout()
        {
            int border = 7;
            int minMixerHeight = 487;


            // todo - if there are multiple screens , need some way of knowing which 
            // screen the mixers should be on 
            var screens = WindowManagerService.GetAllMonitors();
            // this.NumberOfScreens = screens.Count();
            var primaryScreen = screens.Select(x => x.WorkRect)
                .OrderByDescending(r => (long)r.Width * r.Height)
                .FirstOrDefault();

            var mixerLayoutMode = primaryScreen.Height / this.cubaseWindowMonitor.MixerConsoles.Count < minMixerHeight ? MixerLayoutMode.ByWidth : MixerLayoutMode.ByHeight;

            // var taskBarSize = WindowManagerService.GetTaskBarSize();

            primaryScreen.Left = primaryScreen.Left - border;
            primaryScreen.Right = primaryScreen.Right + (border * 2);
            // primaryScreen.Bottom = primaryScreen.Bottom - border;

            var currentTop = primaryScreen.Top;
            var currentLeft = primaryScreen.Left;

            var heightPerWindow = mixerLayoutMode == MixerLayoutMode.ByHeight
                ? primaryScreen.Height / this.cubaseWindowMonitor.MixerConsoles.Count
                : primaryScreen.Height + border;

            var widthPerWindow = mixerLayoutMode == MixerLayoutMode.ByWidth
                ? primaryScreen.Width / this.cubaseWindowMonitor.MixerConsoles.Count
                : primaryScreen.Width;

            var mixerWindows = this.cubaseWindowMonitor.GetMixerWindows();

            mixerWindows.ForEach((mixer) =>
            {
                if (mixer.State == WindowManager.Models.WindowState.Minimized)
                {
                    mixer.Restore();
                }

                // assume height is ok 
                var targetRect = new Rect()
                {
                    Left = primaryScreen.Left,
                    Top = currentTop,
                    Right = primaryScreen.Right,
                    Bottom = currentTop + heightPerWindow
                };

                if (mixerLayoutMode == MixerLayoutMode.ByWidth)
                {
                    targetRect.Left = currentLeft;
                    targetRect.Right = currentLeft + (widthPerWindow + border);
                    targetRect.Top = primaryScreen.Top;
                    targetRect.Bottom = primaryScreen.Bottom;
                }

                mixer.WithPosition(targetRect)
                                .SetPosition()
                                .Refresh();
                currentTop += heightPerWindow;
                currentLeft += (widthPerWindow - border);
            });
            if (mixerWindows.Count == 1)
            {
                mixerWindows[0].Maximise()
                               .Focus();
            }
        }

        private void FocusWindow(string windowName)
        {
            var window = this.cubaseWindowMonitor.CubaseWindows.GetWindowThatStartsWith(windowName);
            if (window != null)
            {
                window.Focus();
            }
        }

        private async Task<bool> WaitForMixConsoleCount(int consoleCount)
        {
            var maxCount = 10000;
            var currentCount = 0;
            while (this.cubaseWindowMonitor.MixerConsoles.Count < consoleCount)
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

    public enum MixerLayoutMode
    {
        ByHeight,
        ByWidth
    }
}
