using Cubase.Midi.Sync.Common.InternalCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.InternalCommands
{
    public interface IInternalCommandControl
    {
        Action<InternalCommand> OnCommand { get; set; }
    }
}
