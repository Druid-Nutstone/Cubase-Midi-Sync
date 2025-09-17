using Cubase.Midi.Sync.Common.Mixer;

namespace Cubase.Midi.Sync.Server.Services.Mixer
{
    public interface IMixerService
    {
        Task<CubaseMixerCollection> MixerCommand(CubaseMixer cubaseMixer);

        Task<CubaseMixerCollection> GetMixer();
    }
}
