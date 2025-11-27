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
        public IEnumerable<string> SupportedKeys => ["Midi"];

        public CubaseMidiService(ILogger<CubaseMidiService> logger, 
                                 IMidiService midiService, 
                                 ICacheService cacheService,
                                 IServiceProvider serviceCollection) 
        { 
            this.logger = logger;
            this.services = serviceCollection;
            this.midiService = midiService;
            this.cacheService = cacheService;
            this.commandCollection = new CubaseMidiCommandCollection(CubaseServerConstants.KeyCommandsFileLocation); 
            
        } 
        
        public CubaseActionResponse ProcessAction(ActionEvent request)
        {
            try
            {
                CubaseActionResponse response = CubaseActionResponse.CreateSuccess();

                var result = this.ExecuteMidiCommand(request.Action);
                if (!result)
                {
                     response = CubaseActionResponse.CreateError($"Could not execute Midi command {request.Action}");
                }; 
                return response;
            }
            catch (Exception ex)
            {
                return CubaseActionResponse.CreateError($"Exception sending key: {ex.Message}");
            }
        }

        public async Task<CubaseActionResponse> ProcessActionAsync(ActionEvent request)
        {
            try
            {
                CubaseActionResponse response = CubaseActionResponse.CreateSuccess();

                var result = await this.ExecuteMidiCommandAsync(request.Action);
                if (!result)
                {
                    response = CubaseActionResponse.CreateError($"Could not execute Midi command {request.Action}");
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
                bool isReady = false;
                this.midiService.OnReadyReceived = () => 
                { 
                    isReady = true;
                };
                this.midiService.SendSysExMessage(MidiCommand.Ready, "{}");
                while (!isReady)
                {
                    Thread.Sleep(50); // Wait for ready signal
                }

                return this.midiService.SendMidiMessage(currentMidiCommand);
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> ExecuteMidiCommandAsync(string midiCommand)
        {
            var currentMidiCommand = this.commandCollection.GetCommandByCommand(midiCommand);
            if (currentMidiCommand != null)
            {
                return this.midiService.SendMidiMessage(currentMidiCommand);
            }
            else
            {
                return false;
            }
        }
    }
}
