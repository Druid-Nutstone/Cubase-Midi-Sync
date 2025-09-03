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
                CubaseActionResponse response = CubaseActionResponse.CreateSuccess();  
                if (request.IsMacro())
                {
                    foreach (var key in request.ActionGroup)
                    {
                        if (!this.SendKey(key, (err) =>
                        {
                            response = CubaseActionResponse.CreateError(err);
                        }));
                    }

                }
                else
                {
                    var result = SendKey(request.Action, (err) => 
                    {
                        response = CubaseActionResponse.CreateError(err);
                    });
                }
                return response;
            }
            catch (Exception ex)
            {
                return CubaseActionResponse.CreateError($"Exception sending key: {ex.Message}");
            }
        }

        private bool SendKey(string key, Action<string> errHandler)
        {
            return this.keyboardService.SendKey(key, errHandler);
        }
    }
}
