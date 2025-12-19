using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Common.Scripts;
using Cubase.Midi.Sync.Common.SysEx;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Midi;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.Server.Services.Windows;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes.Script
{
    public class CubaseScriptService : ICategoryService, ICubaseScriptApi
    {
        private readonly IMidiService midiService;

        private readonly ICacheService cacheService;

        private readonly IServiceProvider serviceProvider;

        private readonly ILogger<CubaseScriptService> logger;

        private MidiChannelCollection? midiTracks;

        private ICubaseWindowMonitor cubaseWindowMonitor;

        public IEnumerable<string> SupportedKeys => ["Script", "SysEx"];

        Dictionary<ScriptFunction, Func<object[], Task<object>>> functions;

        Dictionary<ScriptFunction, Func<object[], Task<ScriptResult>>> commands;

        // In the constructor, fix the dictionary initialization to match the correct types
        public CubaseScriptService(IMidiService midiService,
                                   IServiceProvider serviceProvider,
                                   ILogger<CubaseScriptService> logger,
                                   ICubaseWindowMonitor cubaseWindowMonitor,
                                   ICacheService cacheService)
        {
            this.midiService = midiService;
            this.cacheService = cacheService;
            this.cubaseWindowMonitor = cubaseWindowMonitor; 
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.functions = new Dictionary<ScriptFunction, Func<object[], Task<object>>>
            {
                {ScriptFunction.GetTracks,  this.GetTracks }
            };

            this.commands = new Dictionary<ScriptFunction, Func<object[], Task<ScriptResult>>>()
            {
                {ScriptFunction.DisableRecord, this.DisableRecord },
                {ScriptFunction.EnableRecord, this.EnableRecord },
                {ScriptFunction.SelectTrack, this.SelectTrack },
                {ScriptFunction.ExecuteCommand, this.ExecuteCommand }
            };
        }


        public CubaseActionResponse ProcessAction(ActionEvent request)
        {
            return CubaseActionResponse.CreateError("Non async is NOT supported");
        }

        public async Task<CubaseActionResponse> ProcessActionAsync(ActionEvent request)
        {
            switch (request.CommandType)
            {
                case CubaseAreaTypes.Script:
                    return await RunScript(request);
                case CubaseAreaTypes.SysEx:
                    return await RunSysEx(request);
                default:
                    return CubaseActionResponse.CreateError($"Cannot process {request.CommandType.ToString()}");
            }
        }

        public async Task<CubaseActionResponse> RunSysEx(ActionEvent request)
        {
            // command is in subcommand 
            if (Enum.TryParse<SysExCommand>(request.SubCommand, out var sysExCommand))
            {
                switch (sysExCommand)
                {
                    case SysExCommand.EnableMute:
                    case SysExCommand.DisableMute:
                        this.EnsureTracksArePresent(request.Action, out var muteTracks);
                        var muteResult = await this.midiService.SendSysExMessageAsync(sysExCommand.ToMidiCommand(), muteTracks, 5000);
                        return muteResult ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError("Error enable/disable Mute");
                    case SysExCommand.EnableSolo:
                    case SysExCommand.DisableSolo:
                        this.EnsureTracksArePresent(request.Action, out var soloTracks);
                        var soloResult = await this.midiService.SendSysExMessageAsync(sysExCommand.ToMidiCommand(), soloTracks, 5000);
                        return soloResult ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError("Error enable/disable Solo");
                    case SysExCommand.EnableListen:
                    case SysExCommand.DisableListen:
                        this.EnsureTracksArePresent(request.Action, out var listenTracks);
                        var listenResult = await this.midiService.SendSysExMessageAsync(sysExCommand.ToMidiCommand(), listenTracks, 5000);
                        return listenResult ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError("Error enable/disable Listen");
                    case SysExCommand.DisableAndEnable:
                        var deResult = await DisableRecord([]);
                        if (!deResult.IsSucces)
                        {
                            return CubaseActionResponse.CreateError(deResult.Message);
                        }
                        var enableTracksresult = this.EnsureTracksArePresent(request.Action, out var tracks);
                        if (!enableTracksresult.Success)
                        {
                            return enableTracksresult;
                        }
                        var eResult = await this.EnableRecord(tracks.ToArray());
                        return eResult.IsSucces ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError(eResult.Message);
                    case SysExCommand.DisableRecord:
                        this.EnsureTracksArePresent(request.Action, out var disableTracks);
                        var disableRecordResult = await DisableRecord(disableTracks.ToArray());
                        return disableRecordResult.IsSucces ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError(disableRecordResult.Message);
                    case SysExCommand.EnableRecord:
                        var enabledTracks = this.EnsureTracksArePresent(request.Action, out var enableTracks);
                        if (!enabledTracks.Success)
                        {
                            return enabledTracks;
                        }
                        var enableRecordResult = await EnableRecord(enableTracks.ToArray());
                        return enableRecordResult.IsSucces ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError(enableRecordResult.Message);
                    case SysExCommand.SelectMixerTracks:
                        var mixerTracksRequest = this.EnsureTracksArePresent(request.Action, out var mixerTracks);
                        var mixerTracksResult = await this.SelectMixerTracks(mixerTracks.ToArray(), request.TargetCubaseWindow);
                        return mixerTracksResult.IsSucces ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError(mixerTracksResult.Message);
                    case SysExCommand.SelectTracks:
                        var selectedTracksRequest = this.EnsureTracksArePresent(request.Action, out var selectedTracks);
                        if (!selectedTracksRequest.Success)
                        {
                            return selectedTracksRequest;
                        }
                        var selectedTracksResult = await this.SelectTracks(selectedTracks.ToArray());
                        return selectedTracksResult.IsSucces ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError(selectedTracksResult.Message);
                }
            }

            return CubaseActionResponse.CreateError($"Unknown SysEx command {request.SubCommand} {request.Action}");
        }

        private CubaseActionResponse EnsureTracksArePresent(string trackString, out List<MidiChannel> tracks)
        {
            if (trackString.Contains(';') || !string.IsNullOrEmpty(trackString))
            {
                tracks = this.midiService.MidiChannels.GetTracksWith(trackString.Split(';', StringSplitOptions.RemoveEmptyEntries));
                return tracks.Count > 0 ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError("Notracks have been specified");
            }
            tracks = new List<MidiChannel>();
            return CubaseActionResponse.CreateError("Notracks have been specified");
        }

        public async Task<CubaseActionResponse> RunScript(ActionEvent request)
        {
            CubaseActionResponse actionResponse = CubaseActionResponse.CreateSuccess();
            this.logger.LogInformation($"Processing script {request.Action}");

            if (!File.Exists(request.Action))
            {
                return CubaseActionResponse.CreateError($"Script {request.Action} was not found");
            }

            var fileContent = File.ReadAllLines(request.Action);
            var parser = new ScriptParser();
            var scriptNode = parser.Parse(fileContent, (scriptError) =>
            {
                this.logger.LogError($"Could not parse script {request.Action} {scriptError.Message}");
                actionResponse = CubaseActionResponse.CreateError(scriptError.Message);
            });
            var runner = new ScriptRunner(this);
            var scriptResult = await runner.ExecuteAsync(scriptNode);

            return scriptResult.IsSucces ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError(scriptResult.Message);

        }

        public async Task<ScriptResult> ExecuteCommandAsync(string command, params object[] args)
        {
            var scriptFunction = command.ToScriptFunction();
            if (scriptFunction == ScriptFunction.Unknown)
            {
                return ScriptResult.CreateError($"{command} is not a valid command");
            }
            return await this.commands[scriptFunction](args);
        }

        public async Task<object> CallFunctionAsync(string function, params object[] args)
        {
            var scriptFunction = function.ToScriptFunction();
            if (scriptFunction == ScriptFunction.Unknown)
            {
                return ScriptResult.CreateError($"{function} is not a valid function");
            }
            return await this.functions[scriptFunction](args);
        }

        #region functions
        private async Task<object> GetTracks(object[] args)
        {
            return await Task.Run(() => this.GetChannels(args));
        }
        #endregion

        #region commands 
        private async Task<ScriptResult> ExecuteCommand(object[] args)
        {
            var allCommands = new MidiAndKeysCollection();
            foreach (object arg in args)
            {
                var midiOrKeyCommand = allCommands.GetCommandByName(arg.ToString());
                if (midiOrKeyCommand != null)
                {
                    var processor = this.serviceProvider.GetServices<ICategoryService>().FirstOrDefault(x => x.SupportedKeys.Contains(midiOrKeyCommand.KeyType.ToString()));
                    if (processor == null)
                    {
                        return ScriptResult.CreateError($"No processor found for command {midiOrKeyCommand.Name} {midiOrKeyCommand.KeyType.ToString()}");

                    }
                    var result = await processor.ProcessActionAsync(ActionEvent.CreateFromMidiAndKey(midiOrKeyCommand));
                }
                else
                {
                    return ScriptResult.CreateError($"Cannot find midi or key command {arg.ToString()}");
                }
            }
            return ScriptResult.Create();
        }

        private async Task<ScriptResult> SelectTrack(object[] args)
        {
            List<MidiChannel>? channels = null;
            if (args.IsStringArray())
            {
                channels = GetChannels(args);
            }
            else
            {
                channels = args.ToMidiChannelArray();
            }
            foreach (var channel in channels)
            {
                if (!this.midiService.MidiChannels.HaveTrack(channel.Name))
                {
                    return ScriptResult.CreateError($"The track {channel.Name} does not exist");
                }
            }
            var result = await this.midiService.SelectTracks(channels);
            return result ? ScriptResult.Create() : ScriptResult.CreateError("Could not selected tracks  - timeout");
        }

        private async Task<ScriptResult> DisableRecord(object[] args)
        {
            var channels = args.ToMidiChannelArray();
            var result = await this.midiService.SendSysExMessageAsync(MidiCommand.DisableRecord, channels, 5000);
            return result ? ScriptResult.Create() : ScriptResult.CreateError("Could not execute DisableRecord - timeout");
        }
        private async Task<ScriptResult> EnableRecord(object[] args)
        {
            var channels = args.ToMidiChannelArray();
            var result = await this.midiService.SendSysExMessageAsync(MidiCommand.EnableRecord, channels, 5000);
            return result ? ScriptResult.Create() : ScriptResult.CreateError("Could not execute enable Record - timeout");
        }

        private async Task<ScriptResult> SelectMixerTracks(object[] args, string mixerWindow)
        {
            // deselect all tracks
            var deselectTracksResult = await this.midiService.SendSysExMessageAsync(MidiCommand.DeSelectAll, "", 5000);
            if (!deselectTracksResult)
            {
                return ScriptResult.CreateError("Could not deselect tracks. Might have timed out");
            }
            if (this.cubaseWindowMonitor.WindowExists(mixerWindow))
            {
                if (string.IsNullOrEmpty(mixerWindow))
                {
                    return ScriptResult.CreateError("No mixer window specified");
                };
            }
            else
            {
                return ScriptResult.CreateError($"The mixer window {mixerWindow} does not have focus");  
            }
            var channels = args.ToMidiChannelArray();
            this.logger.LogInformation($"Selecting mixer tracks in window {mixerWindow}: Tracks {String.Join(';',channels.Select(x => x.Name).ToArray())}");  
            if (channels.Count > 1)
            {
                var delay = 160;
                
                // first show all on the current mixer track 
                await this.ExecuteCommand(KnownCubaseMidiCommands.Show_All_Tracks.ToMidiString().ToSingleArray());
                await Task.Delay(delay); // let cubase catch up
                //this.cubaseWindowMonitor.CubaseWindows.GetPrimaryWindow().Focus();
                //await Task.Delay(delay); // let cubase catch up

                // call js to alter required tracks to sel-{trackname}
                var selTracksResult = await this.midiService.SendSysExMessageAsync(MidiCommand.SelectTracks, channels, 5000);
                if (!selTracksResult)
                {
                    return ScriptResult.CreateError("Could not rename tracks. Might have timed out");
                }
                await Task.Delay(delay);
                // call ple to actually select the tracks - 'cos we cant do it programatically 
                var selectTracksResult = await this.ExecuteCommand(KnownCubaseMidiCommands.Select_Mixer_Tracks.ToMidiString().ToSingleArray());
                if (!selectTracksResult.IsSucces)
                {
                    return selectTracksResult;
                }
                await Task.Delay(delay);
                var renameTracksResult = await this.ExecuteCommand(KnownCubaseMidiCommands.Rename_Mixer_Tracks.ToMidiString().ToSingleArray());
                if (!renameTracksResult.IsSucces)
                {
                    return renameTracksResult;
                }
                await Task.Delay(delay);

                //switch back to mixer window 
                this.logger.LogInformation($"Switching focus to mixer window {mixerWindow}");
                this.cubaseWindowMonitor.CubaseWindows.GetWindowByName(mixerWindow)
                                                      .Focus();
                await Task.Delay(delay+10); 
                await this.ExecuteCommand(KnownCubaseMidiCommands.Show_Selected_Tracks.ToMidiString().ToSingleArray());
                await Task.Delay(delay+10); // let cubase catch up
                await this.ExecuteCommand(KnownCubaseMidiCommands.DeSelect_Tracks.ToMidiString().ToSingleArray());
            }
            // just select a single track
            else
            {
                var selTracksResult = await this.midiService.SendSysExMessageAsync(MidiCommand.SelectTracks, channels, 5000);
                if (!selTracksResult)
                {
                    return ScriptResult.CreateError("Could not rename tracks. Might have timed out");
                }
            }
            return ScriptResult.Create();
        }

        private async Task<ScriptResult> SelectTracks(object[] args)
        {
            var channels = args.ToMidiChannelArray();
            // unfold tracks
            var unfoldTracksResult = await this.ExecuteCommand(KnownCubaseMidiCommands.Expand_Tracks.ToMidiString().ToSingleArray());
            if (!unfoldTracksResult.IsSucces)
            {
                return unfoldTracksResult;
            }
            // show all tracks 
            var showAllTracksResult = await this.ExecuteCommand(KnownCubaseMidiCommands.Show_All_Tracks.ToMidiString().ToSingleArray());
            if (!showAllTracksResult.IsSucces)
            {
                return showAllTracksResult;
            }
            // deselect all track
            var deselectTracksResult = await this.midiService.SendSysExMessageAsync(MidiCommand.DeSelectAll, "", 5000);
            if (!deselectTracksResult)
            {
                return ScriptResult.CreateError("Could not deselect tracks. Might have timed out");
            }
            if (channels.Count > 1)
            {
                // call js to alter required tracks to sel-{trackname}
                var selTracksResult = await this.midiService.SendSysExMessageAsync(MidiCommand.SelectTracks, channels, 5000);
                if (!selTracksResult)
                {
                    return ScriptResult.CreateError("Could not rename tracks. Might have timed out");
                }
                // call ple to actually select the tracks - 'cos we cant do it programatically 
                var selectTracksResult = await this.ExecuteCommand(KnownCubaseMidiCommands.Select_Tracks.ToMidiString().ToSingleArray());
                if (!selectTracksResult.IsSucces)
                {
                    return selectTracksResult;
                }
                await Task.Delay(120);
                var renameTracksResult = await this.ExecuteCommand(KnownCubaseMidiCommands.Rename_Mixer_Tracks.ToMidiString().ToSingleArray());
                if (!renameTracksResult.IsSucces)
                {
                    return renameTracksResult;
                }

                await this.midiService.SendSysExMessageAsync(MidiCommand.Tracks, "", 3000);
            }
            // just select a single track
            else
            {
                var selTracksResult = await this.midiService.SendSysExMessageAsync(MidiCommand.SelectTracks, channels, 5000);
                if (!selTracksResult)
                {
                    return ScriptResult.CreateError("Could not rename tracks. Might have timed out");
                }
            }
            return ScriptResult.Create();
        }

        #endregion

        #region internal command helpers
        private List<MidiChannel> GetChannels(object[] args)
        {
            if (args.Length == 0)
            {
                return this.midiService.MidiChannels.ToList();
            }
            else
            {
                return this.midiService.MidiChannels.GetTracksWith(args.ToStringArray());
            }
        }
        #endregion
    }
}
