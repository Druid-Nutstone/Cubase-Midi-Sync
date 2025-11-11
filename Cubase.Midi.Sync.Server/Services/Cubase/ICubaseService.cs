using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Common.WebSocket;

namespace Cubase.Midi.Sync.Server.Services.Cubase
{
    public interface ICubaseService
    {
       Task<CubaseActionResponse> ExecuteActionAsync(CubaseActionRequest request);

        Task<MidiChannelCollection> GetTracks();

        Task<MidiChannelCollection> SetSelectedTrack(MidiChannel midiChannel);

        Task<WebSocketMessage> ExecuteWebSocketAsync(WebSocketMessage request);
    }
}
