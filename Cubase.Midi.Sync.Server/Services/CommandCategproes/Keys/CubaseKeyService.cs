using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Keyboard;
using Cubase.Midi.Sync.Server.Services.Windows;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys
{
    public class CubaseKeyService : ICategoryService
    {
        private readonly IKeyboardService keyboardService;

        private readonly ILogger<CubaseKeyService> logger;

        private readonly ICubaseWindowMonitor cubaseWindowMonitor;

        public IEnumerable<string> SupportedKeys => ["Keys","KeyMacro"];

        public CubaseKeyService(IKeyboardService keyboardService, 
                                ILogger<CubaseKeyService> logger, 
                                ICubaseWindowMonitor cubaseWindowMonitor)
        {
            this.keyboardService = keyboardService;
            this.cubaseWindowMonitor = cubaseWindowMonitor; 
            this.logger = logger;
        }



        public CubaseActionResponse ProcessAction(ActionEvent request)
        {
            try
            {
                var response = CubaseActionResponse.CreateSuccess();
                if (this.cubaseWindowMonitor.HaveAtLeastOneCubaseWindowFocused())
                {
                    var result = SendKey(request.Action, (err) =>
                    {
                        response = CubaseActionResponse.CreateError(err);
                    });
                }
                else
                {
                    response = CubaseActionResponse.CreateError("Either cubase is not running or cannot focus a cubase window");
                }
                return response;
            }
            catch (Exception ex)
            {
                return CubaseActionResponse.CreateError($"Exception sending key: {ex.Message}");
            }
        }

        public async Task<CubaseActionResponse> ProcessActionAsync(ActionEvent request)
        {
            var response = CubaseActionResponse.CreateSuccess();
            await Task.Run(() => 
            { 
               response = this.ProcessAction(request);  
            });
            return response;
        }

        private bool SendKey(string key, Action<string> errHandler)
        {
            this.logger.LogInformation($"Executing Key {key}");
            return this.keyboardService.SendKey(key, errHandler);
        }
    }
}
