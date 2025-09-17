using Cubase.Midi.Sync.Common.Mixer;

namespace Cubase.Midi.Sync.Server.Services.Cache
{
    public class CacheService : ICacheService
    {
        public CubaseMixerCollection CubaseMixer { get; set; } = CubaseMixerCollection.Create();

        public void Initialise()
        {
            // nop
        }
    }
}
