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
using System.Diagnostics;

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

                // Wait for readiness using the async waiter (uses Task.Delay internally) to avoid busy spinning
                var ok = WaitUntilReadyAsync(() => isReady).GetAwaiter().GetResult();
                if (!ok)
                {
                    return false; // timeout or not ready
                }

                return this.midiService.SendMidiMessage(currentMidiCommand);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> WaitUntilReadyAsync(Func<bool> check, int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();

            while (!check())
            {
                if (sw.ElapsedMilliseconds > timeoutMs)
                    return false;

                await Task.Delay(50);
            }

            return true;
        }

        private async Task<bool> ExecuteMidiCommandAsync(string midiCommand)
        {
            var currentMidiCommand = this.commandCollection.GetCommandByCommand(midiCommand);
            if (currentMidiCommand != null)
            {
                // CAN't ASSUME Every midi command will case a command ok  
                //bool isReady = false;
                //this.midiService.onCommandDataHandler = (commandValue) =>
                //{
                //    if (commandValue.Name == currentMidiCommand.Name)
                //    {
                //        isReady = true;
                //    }
                //};
                var result = await this.midiService.SendMidiMessageAsync(currentMidiCommand);
                //bool ok = await WaitUntilReadyAsync(() => isReady);
                //return ok ? result : false;
                return result;
            }
            else
            {
                return false;
            }
        }
    }
}
