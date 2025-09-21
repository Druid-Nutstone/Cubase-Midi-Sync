using Cubase.Midi.Sync.Common.Keys;
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
            var customMenuStrip = new CustomMenuStrip(this.menuStrip, this);
            var areKeysSet = RequiredKeyMappingCollection.Create(null, CubaseConfigurationConstants.KeyCommandsFileLocation);
            if (!areKeysSet.AreAllKeysDefined())
            {
                var keysNotDefined = string.Join(Environment.NewLine, areKeysSet.GetUndefinedKeys().Select(x => x.Category + " " + x.Name));

                MessageBox.Show($"The following Keys Need to be defined in Cubase {Environment.NewLine}{keysNotDefined}", "Missing Keys in Cubase", MessageBoxButtons.OK);
            }
        }
    }
}
