using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Server.Services.Keyboard;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys
{
    public class CubaseKeyService : ICategoryService
    {
        private readonly IKeyboardService keyboardService;

        private readonly ILogger<CubaseKeyService> logger;  

        public CubaseKeyService(IKeyboardService keyboardService, ILogger<CubaseKeyService> logger)
        {
            this.keyboardService = keyboardService; 
            this.logger = logger;
        }

        public CubaseActionResponse ProcessAction(CubaseActionRequest request)
        {
            try
            {
                CubaseActionResponse response = CubaseActionResponse.CreateSuccess();
                this.logger.LogInformation($"Running {request.ButtonType}");
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
            this.logger.LogInformation($"Executing Key {key}");
            return this.keyboardService.SendKey(key, errHandler);
        }
    }
}
