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

        private Dictionary<Type, Func<Node, Task<(bool success, string? message)>>> nodeProcessors;
        private Dictionary<string, Func<IEnumerable<string>, Task<(bool success, string? message)>>> conditionProcessors;

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
            this.InitialiseDictionaries();
        }

        // used for testing 
        //public CubaseScriptService()
        //{
        //    this.InitialiseDictionaries();
        //}

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

            /*
            if (node != null)
            {
                foreach (var statement in node.Statements)
                {
                    var result = await this.nodeProcessors[statement.GetType()]
                                     .Invoke(statement);
                    if (!result.success)
                    {
                        actionResponse = CubaseActionResponse.CreateError($"Script failed on statment {result.message}");
                        break;
                    }
                }
            }
            return actionResponse;
            */
        }

        private void InitialiseDictionaries()
        {
            this.nodeProcessors = new Dictionary<Type, Func<Node, Task<(bool success, string? message)>>>()
            {
                { typeof(IfNode), this.ProcessIF },
                { typeof(CommandNode), this.ProcessCommand }
            };
            // <string, Func<IEnumerable<string>
            this.conditionProcessors = new Dictionary<string, Func<IEnumerable<string>, Task<(bool success, string? message)>>>()
            {
                { ScriptFunction.SelectTrack.ToString().ToLower(), this.ProcessSelectTrack }
            };
        }

        private async Task<(bool success, string? message)> ProcessCommand(Node node)
        {
            var commandNode = (CommandNode)node;
            return (true, null);
        }

        private async Task<(bool success, string? message)> ProcessIF(Node node)
        {
            var ifNode = (IfNode)node;
            (bool success, string? message) conditionResult = (true, null);
            /*
            switch (ifNode.Condition)
            {
                case CommandConditionNode ccn:
                    if (this.conditionProcessors.ContainsKey(ccn.Command.ToLower()))
                    {
                        this.logger.LogInformation($"Processing if node with {ccn.Command}");
                        conditionResult = await this.conditionProcessors[ccn.Command.ToLower()].Invoke(ccn.Args);
                        if (!conditionResult.success)
                        {
                            return conditionResult;
                        }
                    }
                    else
                    {
                        this.logger.LogInformation($"could not find condition {ccn.Command}");
                        conditionResult = (false, $"The condition {ccn.Command} is not recognised");
                        return conditionResult;
                    }
                    break;
            }
            */
            if (conditionResult.success)
            {
                if (ifNode.Then != null)
                {
                    foreach (var thenNode in ifNode.Then)
                    {
                        if (this.nodeProcessors.ContainsKey(thenNode.GetType()))
                        {
                            var resultThen = await this.nodeProcessors[thenNode.GetType()].Invoke(thenNode);
                            if (!resultThen.success)
                            {
                                return resultThen;
                            }
                        }
                    }
                }
            }
            else
            {
                if (ifNode.Else != null)
                {
                    foreach (var elseNode in ifNode.Else)
                    {
                        if (this.nodeProcessors.ContainsKey(elseNode.GetType()))
                        {
                            var resultElse = await this.nodeProcessors[elseNode.GetType()].Invoke(elseNode);
                            if (!resultElse.success)
                            {
                                return resultElse;
                            }
                        }
                    }
                }
            }
            return (true, null);
        }

        private async Task<(bool success, string? message)> ProcessSelectTrack(IEnumerable<string> args)
        {
           
            this.logger.LogInformation($"processing SelectTrack {string.Join(';',args)}");
            //if (args.Count() > 1)
            //{
            //    return (false, "SelectTrack on accepts one argument (the track name)");
            //}

            // make sure the track name exists 
            var tracksToSelect = new List<MidiChannel>();

            var result = await this.GetMidiTracks(); 
            if (!result.success)
            {
                return result;  
            }
            
            foreach (var track in args)
            {
                var targetTrack = midiTracks?.GetChannelByName(track);
                if (targetTrack == null)
                {
                    return (false, $"The track name {track} does not exist in the current cubase project");

                }
                if (targetTrack.Selected.HasValue && !targetTrack.Selected.Value)
                {
                    tracksToSelect.Add(targetTrack);
                }
            } 

            var preCommands = await ExecuteCommands(["Show All Tracks", "Expand Folders"]);
            if (!preCommands.success)
            {
                return preCommands;
            }

            this.midiService.SelectTracks(tracksToSelect);

            // asert alltracks are selected 
            result = await this.GetMidiTracks();
            if (!result.success)
            {
                return result;  
            }
            foreach (var mtrack in tracksToSelect)
            {
                if (!midiTracks.GetChannelByName(mtrack.Name).Selected.Value)
                {
                    return (false, $"Track {mtrack.Name} has NOT been selected");
                }
            }
            return (true, null);

            async Task<(bool success, string? message)> ExecuteCommands(IEnumerable<string> commands)
            {
                foreach (var cmd in commands)
                {
                    this.logger.LogInformation($"Executing {cmd}");
                    var result = await this.RunCubaseMidiAndKeyCommand(cmd);
                    if (!result.success)
                    {
                        return result;
                    }
                }
                return (true, null);
            }
        }

        private async Task<(bool success, string? message)> RunCubaseMidiAndKeyCommand(string name)
        {
            var command = this.cacheService.MidiAndKeys.GetCommandByName(name);

            if (command == null)
            {
                return (false, $"Could not find command {name}");
            }

            var commandResult = await this.ProcessMidiAndKeyCommand(command);

            return (commandResult.Success, commandResult.Message);
        }
         
        private async Task<CubaseActionResponse> ProcessMidiAndKeyCommand(MidiAndKey midiAndKey)
        {
            var actionEvent = ActionEvent.CreateFromMidiAndKey(midiAndKey);
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

        private async Task<(bool success, string? message)> GetMidiTracks()
        {
            (bool success, string? message) result = (true, null);
            midiTracks = await this.midiService.GetTracksAsync((error) => 
            {
                result = (false, error); 
            });
            return result;
        }

        public async Task<ScriptResult> ExecuteCommandAsync(string command, params object[] args)
        {
            throw new NotImplementedException();
        }

        public async Task<object> CallFunctionAsync(string function, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
