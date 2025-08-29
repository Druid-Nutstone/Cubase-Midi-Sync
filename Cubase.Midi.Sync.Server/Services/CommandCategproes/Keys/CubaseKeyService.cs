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
                var keyPressed = this.keyboardService.SendKey(request.Action);
                return keyPressed ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError("Failed to send key event");
            }
            catch (Exception ex)
            {
                return CubaseActionResponse.CreateError($"Exception sending key: {ex.Message}");
            }
        }
    }
}
