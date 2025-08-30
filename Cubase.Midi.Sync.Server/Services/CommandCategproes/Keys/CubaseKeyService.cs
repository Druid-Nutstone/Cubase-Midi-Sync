using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Server.Services.Keyboard;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys
{
    public class CubaseKeyService : ICategoryService
    {
        private readonly IKeyboardService keyboardService;

        public CubaseKeyService(IKeyboardService keyboardService)
        {
            this.keyboardService = keyboardService; 
        }

        public CubaseActionResponse ProcessAction(CubaseActionRequest request)
        {
            try
            {
                if (request.ButtonType == Common.CubaseButtonType.Macro)
                {
                    foreach (var key in request.ActionGroup)
                    {
                        if (!this.SendKey(key))
                        {
                            CubaseActionResponse.CreateError($"Failed to send key event {key}");
                        }
                    }
                    return CubaseActionResponse.CreateSuccess();
                }
                else
                {
                    var result = SendKey(request.Action);
                    return result ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError($"Could not process key {request.Action}");
                }
            }
            catch (Exception ex)
            {
                return CubaseActionResponse.CreateError($"Exception sending key: {ex.Message}");
            }
        }

        private bool SendKey(string key)
        {
            return this.keyboardService.SendKey(key);
        }
    }
}
