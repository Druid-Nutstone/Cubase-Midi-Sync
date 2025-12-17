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

        public Task<CubaseActionResponse> ProcessActionAsync(ActionEvent request)
        {
            if (!this.cubaseWindowMonitor.HaveAtLeastOneCubaseWindowFocused())
                return Task.FromResult(CubaseActionResponse.CreateError(
                    "Either Cubase is not running or cannot focus a Cubase window"));

            var result = SendKey(request.Action, (err) => 
            {
                this.logger.LogInformation($"Error running {request.Action} {err}");
            });

            return Task.FromResult(result
                ? CubaseActionResponse.CreateSuccess()
                : CubaseActionResponse.CreateError("Invalid key or mapping"));
        }

        private bool SendKey(string key, Action<string> errHandler)
        {
            this.logger.LogInformation($"Executing Key {key}");
            return this.keyboardService.SendKey(key, errHandler);
        }
    }
}
