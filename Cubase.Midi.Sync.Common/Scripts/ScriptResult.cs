using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public class ScriptResult
    {
        public ScriptStatus Status { get; set; }
        
        public string Message { get; set; } 

        public bool IsSucces 
        { 
            get 
            {
                return this.Status == ScriptStatus.Success;
            } 
        } 
        
        public static ScriptResult Create()
        {
            return new ScriptResult { Status = ScriptStatus.Success };
        }

        public static ScriptResult CreateError(string message)
        {
            return new ScriptResult
            {
                Status = ScriptStatus.Error,
                Message = message
            };
        }
    }


    public enum ScriptStatus
    {
        Success,
        Error
    }
}
