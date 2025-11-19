using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes
{
    public interface ICategoryService
    {
        IEnumerable<string> SupportedKeys { get; }
        
        CubaseActionResponse ProcessAction(ActionEvent request);

        Task<CubaseActionResponse> ProcessActionAsync(ActionEvent request);
    }
}
