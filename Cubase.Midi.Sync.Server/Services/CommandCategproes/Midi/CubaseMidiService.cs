using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys;
using Cubase.Midi.Sync.Server.Services.Midi;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes.Midi
{
    public class CubaseMidiService : ICategoryService
    {
        private readonly ILogger<CubaseMidiService> logger; 
        
        private readonly IMidiService midiService;
        
        private readonly ICacheService cacheService;    

        private readonly IServiceProvider services;

        private CubaseMidiCommandCollection commandCollection;

        private ICategoryService keyService;

        public CubaseMidiService(ILogger<CubaseMidiService> logger, 
                                 IMidiService midiService, 
                                 ICacheService cacheService,
                                 IServiceProvider serviceCollection) 
        { 
            this.logger = logger;
            this.services = serviceCollection;
            this.keyService = this.services.GetKeyedService<ICategoryService>(CubaseServiceConstants.KeyService);
            this.midiService = midiService;
            this.cacheService = cacheService;
            this.commandCollection = new CubaseMidiCommandCollection(CubaseServerConstants.KeyCommandsFileLocation); 
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
                        this.logger.LogInformation("Executing Midi command {key} for macro {request}", key, request.ButtonType);
                        var result = this.ExecuteMidiCommand(key);
                        if (!result)
                        {
                             response = CubaseActionResponse.CreateError($"Could not execute Midi command request. {key} {request.Category}");
                             break;
                        }
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    var result = this.ExecuteMidiCommand(request.Action);
                    if (!result)
                    {
                        response = CubaseActionResponse.CreateError($"Could not execute Midi command {request.Action}");
                    }; 
                }
                return response;
            }
            catch (Exception ex)
            {
                return CubaseActionResponse.CreateError($"Exception sending key: {ex.Message}");
            }
        }

        private bool ExecuteMidiCommand(string midiCommand)
        {
            var currentMidiCommand = this.commandCollection.GetCommandByCommand(midiCommand);
            if (currentMidiCommand != null)
            {
                return this.midiService.SendMidiMessage(currentMidiCommand);
            }
            else
            {
                return this.keyService.ProcessAction(CubaseActionRequest.Create(midiCommand)).Success; 
            }
        }
    }
}
