using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using System.Diagnostics;
using Cubase.Midi.Sync.Server.Extensions;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Server.Services.Commands;
using Cubase.Midi.Sync.Server.Services.Mixer;
using Cubase.Midi.Sync.Common.Mixer;

namespace Cubase.Midi.Sync.Server.Services.Cubase
{
    public class CubaseService : ICubaseService
    {
        private readonly IServiceProvider serviceProvider;

        private readonly ILogger<CubaseService> logger;

        private readonly IMidiService midiService;

        private readonly ICommandService commandService;

        private readonly IMixerService mixerService;    

        private CubaseMidiCommandCollection cubaseMidiCommands;



        public CubaseService(IServiceProvider serviceProvider, 
                             ILogger<CubaseService> logger, 
                             IMidiService midiService, 
                             IMixerService mixerService,
                             ICommandService commandService)
        {
            this.serviceProvider = serviceProvider; 
            this.midiService = midiService;
            this.commandService = commandService;
            this.mixerService = mixerService;   
            this.logger = logger;   
            this.cubaseMidiCommands = new CubaseMidiCommandCollection(CubaseServerConstants.KeyCommandsFileLocation);
        }

        public async Task<WebSocketMessage> ExecuteWebSocket(WebSocketMessage request)
        {
            switch (request.Command)
            {
                case WebSocketCommand.Commands:
                    var commands = await commandService.GetCommands();
                    return WebSocketMessage.Create(WebSocketCommand.Commands, commands);
                case WebSocketCommand.ExecuteCubaseAction:
                    var actionRequest = request.GetMessage<CubaseActionRequest>();
                    var actionResponse = await ExecuteAction(actionRequest);
                    if (actionResponse.Success)
                    {
                        return WebSocketMessage.Create(WebSocketCommand.Success);
                    }
                    else
                    {
                        return WebSocketMessage.CreateError(actionResponse.Message);
                    }
                case WebSocketCommand.Mixer:
                    var mixerCommands = await mixerService.MixerCommand(request.GetMessage<CubaseMixer>());
                    return WebSocketMessage.Create(WebSocketCommand.Mixer, mixerCommands);
                default:
                    return WebSocketMessage.CreateError("Unknown command: " + request.Command.ToString());
            }
        }

        public async Task<CubaseActionResponse> ExecuteAction(CubaseActionRequest request)
        {
            if (!await EnsureCubaseIsActive())
            {
                var respone = new CubaseActionResponse
                {
                    Success = false,
                    Message = "Cubase is not running or not the active window."
                };
                this.logger.LogError("Cubase is not running so can't execute {request}", request);
                return respone;
            }
            
            if (request.IsMacro())
            {
                foreach (var cmd in request.ActionGroup)
                {
                    var result = ProcessAction(cmd);
                    if (!result.Success)
                    {
                        return result;
                    }
                    await Task.Delay(50);
                }
                return CubaseActionResponse.CreateSuccess();
            }
            else
            {
                return ProcessAction(request.Action);
            }
        }

        public async Task<MidiChannelCollection> GetTracks()
        {
            return await Task.Run(this.midiService.GetChannels);
        }


        public async Task<MidiChannelCollection> SetSelectedTrack(MidiChannel midiChannel)
        {

                var targetTrack = this.midiService.MidiChannels.GetChannelByName(midiChannel.Name);
                var currentTrack = this.midiService.MidiChannels.GetSelectedTrack();

                if (targetTrack != null && currentTrack != null)
                {
                   if (targetTrack.Index > currentTrack.Index)
                   {
                       var steps = targetTrack.Index - currentTrack.Index;
                       for (int i = 0; i < steps; i++)
                       {
                           this.midiService.SendMidiMessage(this.cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Next_Track));
                           await Task.Delay(20); // give Cubase time to process   
                       }
                   }
                   else if (targetTrack.Index < currentTrack.Index)
                   {
                       var steps = currentTrack.Index - targetTrack.Index;
                       for (int i = 0; i < steps; i++)
                       {
                           this.midiService.SendMidiMessage(this.cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Previous_Track));
                           await Task.Delay(20); // give Cubase time to process   
                       }
                    }
                }
                return this.midiService.MidiChannels;


        }

        private async Task<bool> EnsureCubaseIsActive()
        {
            return await Task.Run(() =>
            {
                var cubase = CubaseExtensions.GetCubaseService();
                return cubase != null;
            });
        }

        private CubaseActionResponse ProcessAction(ActionEvent actionEvent)
        {
            var processor = this.serviceProvider.GetRequiredKeyedService<ICategoryService>(actionEvent.CommandType.ToString());
            if (processor == null)
            {
                return new CubaseActionResponse
                {
                    Success = false,
                    Message = $"No service found for area {actionEvent.CommandType.ToString()}"
                };
            }
            return processor.ProcessAction(actionEvent);
        }
    }
}
