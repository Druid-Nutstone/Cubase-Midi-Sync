using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.Midi;

namespace Cubase.Midi.Sync.Server.Services.Mixer
{
    public class MixerService : IMixerService
    {
        private readonly ICacheService cacheService;    

        private readonly IMidiService midiService;
        
        private readonly IServiceProvider services;  
        
        private readonly ILogger<MixerService> logger;

        private ICategoryService categoryService;   

        public MixerService(ILogger<MixerService> logger,ICacheService cacheService, IMidiService midiService, IServiceProvider services)
        {
           this.cacheService = cacheService;    
           this.midiService = midiService;
            this.logger = logger;   
           this.services = services;
           this.categoryService = this.services.GetKeyedService<ICategoryService>(CubaseServiceConstants.KeyService);
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
                this.categoryService.ProcessAction(CubaseActionRequest.Create(cubaseMixer.KeyAction));
            }
            else
            {
                var commands = cacheService.CubaseMixer.GetCommands(cubaseMixer.Command);
                foreach (var cmd in commands)
                {
                    this.midiService.SendMidiMessage(cmd);
                    await Task.Delay(50); // Small delay to ensure Cubase processes the command
                }
            }
            return cacheService.CubaseMixer;
        }
    }
}
