using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Server.Constants;

namespace Cubase.Midi.Sync.Server.Services.Cache
{
    public class CacheService : ICacheService
    {
        public CubaseMixerCollection CubaseMixer { get; private set; }

        private ILogger<CacheService> logger;   

        public CacheService(ILogger<CacheService> logger) 
        { 
            this.logger = logger;   
        }
        
        public void Initialise()
        {
            this.CubaseMixer = CubaseMixerCollection.Create((msg) => 
            {
                this.logger.LogInformation(msg);
            }, CubaseServerConstants.KeyCommandsFileLocation);
        }
    }
}
