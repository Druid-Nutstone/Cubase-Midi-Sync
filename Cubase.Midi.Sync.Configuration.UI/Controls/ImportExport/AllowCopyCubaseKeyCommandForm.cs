using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.ImportExport
{
    public partial class AllowCopyCubaseKeyCommandForm : Form
    {
        private string cubaseArea;

        private string cubaseKey;
        
        public AllowCopyCubaseKeyCommandForm()
        {
            InitializeComponent();
        }

        public AllowCopyCubaseKeyCommandForm(string cubaseArea, string cubaseKey)
        {
            InitializeComponent();
            this.cubaseArea = cubaseArea;
            this.cubaseKey = cubaseKey;
            this.CubaseAreaCopyButton.Click += CubaseAreaCopyButton_Click;
            this.CubaseKeyCopyButton.Click += CubaseKeyCopyButton_Click;
            this.OKButton.Click += OKButton_Click;
            this.CubaseArea.Text = cubaseArea;  
            this.CubaseKey.Text = cubaseKey;    
        }

        private void OKButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void CubaseKeyCopyButton_Click(object? sender, EventArgs e)
        {
            Clipboard.SetText(cubaseKey);  
        }

        private void CubaseAreaCopyButton_Click(object? sender, EventArgs e)
        {
            Clipboard.SetText(cubaseArea);
        }
    }
}
