using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes
{
    public interface ICategoryService
    {
        IEnumerable<string> SupportedKeys { get; }

        Task<CubaseActionResponse> ProcessActionAsync(ActionEvent request);
    }
}
