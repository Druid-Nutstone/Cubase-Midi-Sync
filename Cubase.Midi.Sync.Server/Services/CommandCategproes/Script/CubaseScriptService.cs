using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Common.Scripts;
using Cubase.Midi.Sync.Server.Services.Midi;

namespace Cubase.Midi.Sync.Server.Services.CommandCategproes.Script
{
    public class CubaseScriptService : ICategoryService
    {
        private readonly IMidiService midiService;
        
        public IEnumerable<string> SupportedKeys => ["Script"];

        private MidiChannelCollection midiChannels;

        // Change the type of nodeProcessors to use Type as the key and Func<Node, bool> as the value
        private Dictionary<Type, Func<Node, (bool success, string? message)>> nodeProcessors;

        private Dictionary<string, Func<IEnumerable<string>, (bool success, string? message)>> conditionProcessors;

      // In the constructor, fix the dictionary initialization to match the correct types
        public CubaseScriptService(IMidiService midiService)
        {
            this.midiService = midiService;
            midiService.RegisterOnChannelChanged(this.GetCubaseTracks);
            this.InitialiseDictionaries();
        }

        // used for testing 
        public CubaseScriptService()
        {
            this.InitialiseDictionaries();
        }

        public CubaseActionResponse ProcessAction(ActionEvent request)
        {
            CubaseActionResponse actionResponse = null;
            var fileName = this.FindScript(request.Action);
            if (fileName == null)
            {
                return CubaseActionResponse.CreateError($"Script {request.Action} was not found");
            }
            var fileContent = File.ReadAllLines(fileName);
            var parser = new CommandParser();
            var node = parser.Parse(fileContent, (scriptError) =>
            {
                actionResponse = CubaseActionResponse.CreateError(scriptError.Message);
            });
            if (node != null)
            {
                foreach (var statement in node.Statements)
                {
                    var result = this.nodeProcessors[statement.GetType()]
                                     .Invoke(statement);   
                    if (!result.success)
                    {
                        actionResponse = CubaseActionResponse.CreateError($"Script failed on statment {result.message}");
                        break;
                    }
                }
            }
            return CubaseActionResponse.CreateSuccess();
        }

        public async Task<CubaseActionResponse> ProcessActionAsync(ActionEvent request)
        {
            return await Task.Run(() =>
            {
                return this.ProcessAction(request);
            });
        }

        private void InitialiseDictionaries()
        {
            this.nodeProcessors = new Dictionary<Type, Func<Node, (bool success, string message)>>()
            {
                { typeof(IfNode), this.ProcessIF },
                { typeof(CommandNode), this.ProcessCommand }
            };

            this.conditionProcessors = new Dictionary<string, Func<IEnumerable<string>, (bool success, string message)>>()
            {
                { "selecttrack", this.ProcessSelectTrack }
            };
        }

        private string? FindScript(string scriptName)
        {
            if (!Path.HasExtension(scriptName))
            {
                scriptName += CubaseServerSettings.ScriptExtension;
            }
            return Directory.EnumerateFiles(CubaseServerSettings.ScriptPath, "*", SearchOption.AllDirectories)
                            .FirstOrDefault(file =>
                                Path.GetFileName(file).Equals(scriptName, StringComparison.OrdinalIgnoreCase));
        }

        private (bool success, string? message) ProcessCommand(Node node)
        {
            var commandNode = (CommandNode)node;
            return (true, null);
        }

        private (bool success, string? message) ProcessIF(Node node)
        {
            var ifNode = (IfNode)node;
            (bool success, string? message) conditionResult = (true, null); 
            switch (ifNode.Condition)
            {
                case CommandConditionNode ccn:
                    if (this.conditionProcessors.ContainsKey(ccn.Command.ToLower()))
                    {
                        conditionResult = this.conditionProcessors[ccn.Command.ToLower()].Invoke(ccn.Args);
                    }
                    else
                    {
                        conditionResult= (false, $"The condition {ccn.Command} is not recognised");
                    }
                    break;
            }
            if (conditionResult.success)
            {
                if (ifNode.Then != null)
                {
                    foreach (var thenNode in ifNode.Then)
                    {
                        if (this.nodeProcessors.ContainsKey(thenNode.GetType()))
                        {
                            var resultThen = this.nodeProcessors[thenNode.GetType()].Invoke(thenNode);
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
                            var resultElse = this.nodeProcessors[elseNode.GetType()].Invoke(elseNode);
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

        private (bool success, string? message) ProcessSelectTrack(IEnumerable<string> args)
        {
            if (args.Count() > 1)
            {
                return (false, "SelectTrack on accepts one argument (the track name)");
            }

            var cubaseCommands = new MidiAndKeysCollection();

            var deselectCommand = cubaseCommands.GetCommandByName("DeSelect Tracks");

            var trackName = args.First();
            var channelName = this.midiChannels.GetChannelByName(trackName);
            if (channelName == null)
            {
                return (false, $"The track name {trackName} does not exist in the current cubase project");
            }

            return (true, null);
        }

        private void GetCubaseTracks(MidiChannelCollection midiChannels)
        {
            this.midiChannels = midiChannels;
        }
    }
}
