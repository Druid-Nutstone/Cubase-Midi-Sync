using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cubase.Midi.Sync.Server.Tests.Tests.ScriptParsers
{
    public class TestCubaseApi : ICubaseScriptApi
    {
        Dictionary<ScriptFunction, Func<object[], Task<object>>> functions;

        Dictionary<ScriptFunction, Func<object[], Task<ScriptResult>>> commands;
        
        public TestCubaseApi()
        {
            this.functions = new Dictionary<ScriptFunction, Func<object[], Task<object>>>
            {
                {ScriptFunction.GetTracks,  this.GetTracks }
            };

            this.commands = new Dictionary<ScriptFunction, Func<object[], Task<ScriptResult>>>()
            {
                {ScriptFunction.DisableRecord, this.DisableRecord },
                {ScriptFunction.EnableRecord, this.EnableRecord }
            };
        }

        public Task<object> CallFunctionAsync(string function, params object[] args)
        {
            var scriptFunction = function.ToScriptFunction();
            if (scriptFunction == ScriptFunction.Unknown)
            {
                return Task.FromResult((object)ScriptResult.CreateError($"{function} is not a valid function"));
            }
            return this.functions[scriptFunction](args);
        }

        public Task<ScriptResult> ExecuteCommandAsync(string command, params object[] args)
        {
            var scriptFunction = command.ToScriptFunction();
            if (scriptFunction == ScriptFunction.Unknown)
            {
                return Task.FromResult(ScriptResult.CreateError($"{command} is not a valid command"));
            }
            return this.commands[scriptFunction](args);
        }


        #region functions
        private async Task<object> GetTracks(object[] args) 
        {
            if (args.Count() > 0)
            {

            }
            
            return Task.CompletedTask;
        }
        #endregion

        #region commands 
        private async Task<ScriptResult> DisableRecord(object[] args)
        {
            return ScriptResult.Create();
        }
        private async Task<ScriptResult> EnableRecord(object[] args)
        {
            return ScriptResult.Create();
        }
        #endregion

        #region helper functions
        private MidiChannelCollection GetTracks
    }
}
