using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Mixer;

namespace Cubase.Midi.Sync.Server.Services.Cache
{
    public interface ICacheService
    {
        void Initialise();

        CubaseMixerCollection CubaseMixer { get; }
    }
}
