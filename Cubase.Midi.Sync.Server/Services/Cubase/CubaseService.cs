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
using Cubase.Midi.Sync.WindowManager.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Windows;
using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.WebSockets;
using Cubase.Midi.Sync.Common.Tracks;

namespace Cubase.Midi.Sync.Server.Services.Cubase
{
    public class CubaseService : ICubaseService
    {
        private readonly IServiceProvider serviceProvider;

        private readonly ILogger<CubaseService> logger;

        private readonly IMidiService midiService;

        private readonly ICommandService commandService;

        private readonly IMixerService mixerService;

        private readonly ICacheService cacheService;

        private readonly ICubaseWindowMonitor cubaseWindowMonitor;

        private CubaseMidiCommandCollection cubaseMidiCommands;

        public CubaseService(IServiceProvider serviceProvider, 
                             ILogger<CubaseService> logger, 
                             IMidiService midiService, 
                             ICacheService cacheService,
                             IMixerService mixerService,
                             ICubaseWindowMonitor cubaseWindowMonitor,
                             ICommandService commandService)
        {
            this.serviceProvider = serviceProvider; 
            this.midiService = midiService;
            this.commandService = commandService;
            this.cacheService = cacheService;
            this.cubaseWindowMonitor = cubaseWindowMonitor;
            this.mixerService = mixerService;   
            this.logger = logger;   
            this.cubaseMidiCommands = new CubaseMidiCommandCollection(CubaseServerConstants.KeyCommandsFileLocation);
        }

        public async Task<WebSocketMessage> ExecuteWebSocketAsync(WebSocketMessage request)
        {
            switch (request.Command)
            {
                case WebSocketCommand.SysEx:
                    
                    return WebSocketMessage.Create(WebSocketCommand.Success);
                case WebSocketCommand.TrackState:
                    return WebSocketMessage.Create(WebSocketCommand.TrackState, TrackState.CreateFromChannels(await this.GetTracks()));
                case WebSocketCommand.SelectTracks:
                    var tracksToSelect = request.GetMessage<List<MidiChannel>>();
                    this.midiService.SelectTracks(tracksToSelect);
                    await this.GetTracks();
                    return WebSocketMessage.Create(WebSocketCommand.Success);
                case WebSocketCommand.Tracks:
                    await this.GetTracks();
                    return WebSocketMessage.Create(WebSocketCommand.Success);
                case WebSocketCommand.Commands:
                    var commands = await commandService.GetCommands();
                    return WebSocketMessage.Create(WebSocketCommand.Commands, commands);
                case WebSocketCommand.ExecuteCubaseAction:
                    var actionRequest = request.GetMessage<CubaseActionRequest>();
                    var actionResponse = await ExecuteActionAsync(actionRequest);
                    if (actionResponse.Success)
                    {
                        return WebSocketMessage.Create(WebSocketCommand.Success);
                    }
                    else
                    {
                        return WebSocketMessage.CreateError(actionResponse.Message);
                    }
                case WebSocketCommand.Mixer:
                    var mixerRequest = request.GetMessage<CubaseMixerRequest>();
                    var mixerResponse = await mixerService.MixerRequest(mixerRequest);
                    if (!string.IsNullOrEmpty(mixerResponse.Error))
                    {
                        return WebSocketMessage.CreateError(mixerResponse.Error);   
                    }
                    return WebSocketMessage.Create(WebSocketCommand.Mixer, mixerResponse);
                case WebSocketCommand.Windows:
                    return WebSocketMessage.Create(WebSocketCommand.Windows, cubaseWindowMonitor.CubaseWindows); ;
                default:
                    return WebSocketMessage.CreateError("Unknown command: " + request.Command.ToString());
            }
        }

        public async Task<CubaseActionResponse> ExecuteActionAsync(CubaseActionRequest request)
        {
            var primaryWindow = EnsureCubaseIsActive();
            if (primaryWindow == null)
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
                    var result = await ProcessActionAsync(cmd);
                    if (!result.Success)
                    {
                        return result;
                    }
                    await Task.Delay(40);
                }
                return CubaseActionResponse.CreateSuccess();
            }
            else
            {
                return await ProcessActionAsync(request.Action);
            }
        }

        public async Task<MidiChannelCollection> GetTracks()
        {
            return await this.midiService.GetTracksAsync((error) => 
            {
                this.logger.LogError($"could not execute GetTracks. Error from midi service {error}");
            });
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
                       }
                   }
                   else if (targetTrack.Index < currentTrack.Index)
                   {
                       var steps = currentTrack.Index - targetTrack.Index;
                       for (int i = 0; i < steps; i++)
                       {
                           this.midiService.SendMidiMessage(this.cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Previous_Track));
                       }
                    }
                }
                return this.midiService.MidiChannels;


        }

        private WindowPosition EnsureCubaseIsActive()
        {
            return this.cubaseWindowMonitor.CubaseWindows.GetPrimaryWindow();
        }

        private async Task<CubaseActionResponse> ProcessActionAsync(ActionEvent actionEvent)
        {
            var processor = this.serviceProvider.GetServices<ICategoryService>().FirstOrDefault(x => x.SupportedKeys.Contains(actionEvent.CommandType.ToString()));
            if (processor == null)
            {
                return new CubaseActionResponse
                {
                    Success = false,
                    Message = $"No service found for area {actionEvent.CommandType.ToString()}"
                };
            }
            return await processor.ProcessActionAsync(actionEvent);
        }
    }
}
