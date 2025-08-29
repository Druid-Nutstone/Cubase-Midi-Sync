using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using System.Diagnostics;
using Cubase.Midi.Sync.Server.Extensions;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;

namespace Cubase.Midi.Sync.Server.Services.Cubase
{
    public class CubaseService : ICubaseService
    {
        private readonly IServiceProvider serviceProvider;

        public CubaseService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider; 
        }

        public async Task<CubaseActionResponse> ExecuteAction(CubaseActionRequest request)
        {
            if (!await EnsureCubaseIsActive())
            {
                return new CubaseActionResponse
                {
                    Success = false,
                    Message = "Cubase is not running or not the active window."
                };
            }
            // locate the service processor 
            var catgeoryService = this.serviceProvider.GetKeyedService<ICategoryService>(request.Category);
            


            if (catgeoryService == null)
            {
                return new CubaseActionResponse
                {
                    Success = false,
                    Message = $"No service found for area {request.Category}"
                };
            }
            // not async because neither midi or key commands are async operations
            return catgeoryService.ProcessAction(request);    
        }




        private async Task<bool> EnsureCubaseIsActive()
        {
            return await Task.Run(() =>
            {
                var cubase = CubaseExtensions.GetCubaseService();
                return cubase != null;
            });
        }
    }
}
