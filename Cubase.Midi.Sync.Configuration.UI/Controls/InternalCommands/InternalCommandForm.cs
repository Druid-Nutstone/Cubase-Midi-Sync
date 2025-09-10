using Cubase.Midi.Sync.Common.InternalCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.InternalCommands
{
    public partial class InternalCommandForm : Form
    {
        private Action<InternalCommand> OnCommand { get; set; }

        private InternalCommandsCollection internalCommands = new InternalCommandsCollection(); 

        public InternalCommandForm()
        {
            InitializeComponent();
        }

        public InternalCommandForm(Action<InternalCommand> onCommand)
        {
            InitializeComponent();
            this.CommandComboBox.Items.Clear();
            this.CommandComboBox.Items.AddRange(internalCommands.Select(x => x.CommandType.ToString()).ToArray());
            this.CommandComboBox.SelectedIndexChanged += CommandComboBox_SelectedIndexChanged;
            this.OnCommand = onCommand; 
        }

        private void CommandComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedCommand = Enum.Parse<InternalCommandType>(this.CommandComboBox.SelectedItem.ToString());
            switch (selectedCommand)
            {
                case InternalCommandType.Navigate:
                    this.LoadDataControl(new NavigateInternalCommandControl());
                    break;
                default:
                    break;
            }
        }

          

        private void LoadDataControl(Control cntrl)
        {
            this.CommandPanel.Controls.Clear(); 
            cntrl.Dock = DockStyle.Fill;    
            this.CommandPanel.Controls.Add(cntrl);
            ((IInternalCommandControl)cntrl).OnCommand = (command) =>
            {
                this.OnCommand(command);
                this.Close();    
            };  
        }
    }
}
