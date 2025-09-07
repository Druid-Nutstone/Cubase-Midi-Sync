using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Forms.CubaseCommands
{
    public partial class CubaseCommandsForm : Form
    {
        public CubaseCommandsForm()
        {
            InitializeComponent();
            this.Controls.Clear();
            this.Controls.Add(new CubaseCommandsControl());
        }
    }
}
