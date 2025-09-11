using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;

namespace Cubase.Midi.Sync.Server.Services.Cubase
{
    public interface ICubaseService
    {
       Task<CubaseActionResponse> ExecuteAction(CubaseActionRequest request);

        Task<MidiChannelCollection> GetTracks();
    }
}
