using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Extensions
{
    public static class ActionExtensions
    {
        public static List<ActionEvent> Clone(this List<ActionEvent> actions)
        {
            var newActions = new List<ActionEvent>();  
            foreach (var action in actions)
            {
                newActions.Add(action.Clone());
            }
            return newActions;
        }
    }
}
