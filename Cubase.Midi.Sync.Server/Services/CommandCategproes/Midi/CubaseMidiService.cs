using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.Midi;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes.Midi
{
    public class CubaseMidiService : ICategoryService
    {
        private readonly ILogger<CubaseMidiService> logger; 
        
        private readonly IMidiService midiService;
        
        private CubaseMidiCommandCollection commandCollection;

        public CubaseMidiService(ILogger<CubaseMidiService> logger, IMidiService midiService) 
        { 
            this.logger = logger;   
            this.midiService = midiService;
            this.commandCollection = new CubaseMidiCommandCollection(); 
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
            return false;
        }
    }
}
