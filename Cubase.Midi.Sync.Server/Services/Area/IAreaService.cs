using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;

namespace Cubase.Midi.Sync.Server.Services.Area
{
    public interface IAreaService
    {
        CubaseActionResponse ProcessAction(CubaseActionRequest request);
    }
}
