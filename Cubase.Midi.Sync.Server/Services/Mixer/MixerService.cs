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

        private ICategoryService categoryService;   

        public MixerService(ICacheService cacheService, IMidiService midiService, IServiceProvider services)
        {
           this.cacheService = cacheService;    
           this.midiService = midiService;
           this.services = services;
           this.categoryService = this.services.GetKeyedService<ICategoryService>(CubaseServiceConstants.KeyService);
        }

        public async Task<CubaseMixerCollection> GetMixer()
        {
            await Task.Delay(1);
            this.cacheService.CubaseMixer = CubaseMixerCollection.Create();
            return this.cacheService.CubaseMixer;
        }

        public async Task<CubaseMixerCollection> MixerCommand(CubaseMixer cubaseMixer)
        {
            if (!string.IsNullOrEmpty(cubaseMixer.KeyAction))
            {
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
