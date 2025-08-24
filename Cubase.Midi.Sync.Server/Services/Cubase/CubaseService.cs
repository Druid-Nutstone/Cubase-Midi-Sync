using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Area;
using System.Diagnostics;
using Cubase.Midi.Sync.Server.Extensions;

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
            var areaService = this.serviceProvider.GetKeyedService<IAreaService>(request.Area);
            


            if (areaService == null)
            {
                return new CubaseActionResponse
                {
                    Success = false,
                    Message = $"No service found for area {request.Area}"
                };
            }
            // not async because neither midi or key commands are async operations
            return areaService.ProcessAction(request);    
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
