using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.Midi;

namespace Cubase.Midi.Sync.Server.Services.Mixer
{
    public class MixerService : IMixerService
    {
        private readonly ICacheService cacheService;    

        private readonly IMidiService midiService;  

        public MixerService(ICacheService cacheService, IMidiService midiService)
        {
           this.cacheService = cacheService;    
           this.midiService = midiService;
        }

        public async Task<CubaseMixerCollection> GetMixer()
        {
            await Task.Delay(1);
            this.cacheService.CubaseMixer = CubaseMixerCollection.Create();
            return this.cacheService.CubaseMixer;
        }

        public async Task<CubaseMixerCollection> MixerCommand(CubaseMixer cubaseMixer)
        {
            var commands = cacheService.CubaseMixer.GetCommands(cubaseMixer.Command);
            foreach (var cmd in commands)
            {
                this.midiService.SendMidiMessage(cmd);  
                await Task.Delay(50); // Small delay to ensure Cubase processes the command
            }
            return cacheService.CubaseMixer;
        }
    }
}
