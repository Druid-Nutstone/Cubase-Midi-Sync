using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Common.Scripts;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Midi;
using Cubase.Midi.Sync.Server.Services.Midi;
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

        public IEnumerable<string> SupportedKeys => ["Script"];

        Dictionary<ScriptFunction, Func<object[], Task<object>>> functions;

        Dictionary<ScriptFunction, Func<object[], Task<ScriptResult>>> commands;

        // In the constructor, fix the dictionary initialization to match the correct types
        public CubaseScriptService(IMidiService midiService,
                                   IServiceProvider serviceProvider,
                                   ILogger<CubaseScriptService> logger,
                                   ICacheService cacheService)
        {
            this.midiService = midiService;
            this.cacheService = cacheService;
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
            return this.GetChannels(args);
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
