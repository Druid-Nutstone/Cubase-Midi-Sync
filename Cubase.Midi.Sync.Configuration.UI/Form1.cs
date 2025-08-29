using Cubase.Midi.Sync.Configuration.UI.Controls;

namespace Cubase.Midi.Sync.Configuration.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DataPanel.Controls.Clear();
            this.DataPanel.Controls.Add(new MainTabControl());  
        }
    }
}
