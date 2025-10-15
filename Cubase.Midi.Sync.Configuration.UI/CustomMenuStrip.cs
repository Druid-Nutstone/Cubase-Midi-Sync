using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Configuration.UI.Forms;
using Cubase.Midi.Sync.Configuration.UI.Forms.CubaseCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI
{
    public class CustomMenuStrip
    {
        private readonly MenuStrip menu; 
        
        private readonly Form1 form;

        public CustomMenuStrip(MenuStrip menu, Form1 form)
        {
            this.menu = menu;
            this.form = form;
            this.Initialise();
        }

        private void Initialise()
        {
            this.menu.Items.Clear();
            this.menu.Items.Add(new FileMenuStripItem(this.form));
            this.menu.Items.Add(new UtilityMenuStripItem());
            this.menu.Items.Add(new ExportMenuStripItem());
            this.menu.Items.Add(new ImportMenuStripItem());
        }
    }

    public class ShowLogFileItem : ToolStripMenuItem
    {
        public ShowLogFileItem()
        {
            this.Text = "Show Log File";
        }
        protected override void OnClick(EventArgs e)
        {
            var logFilePath = new CubaseServerSettings().CubseServerLogPath;
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "notepad.exe";
            p.StartInfo.Arguments = logFilePath;
            p.Start();
        }
    }

    public class ImportMenuStripItem : ToolStripMenuItem
    {
        public ImportMenuStripItem()
        {
            this.Text = "Import";
            this.DropDownItems.Add(new ImportButtonsItem());
        }
    }

    public class ImportButtonsItem : ToolStripMenuItem
    {
        public ImportButtonsItem()
        {
            this.Text = "Buttons";

        }

        protected override void OnClick(EventArgs e)
        {
            var importForm = new ImportButtonsForm();
            importForm.ShowDialog();
        }
    }

    public class ExportMenuStripItem : ToolStripMenuItem
    {
        public ExportMenuStripItem()
        {
            this.Text = "Export";
            this.DropDownItems.Add(new ExportButtonsItem());
        }
    }

    public class ExportButtonsItem : ToolStripMenuItem
    {
        public ExportButtonsItem()
        {
            this.Text = "Buttons";
            
        }

        protected override void OnClick(EventArgs e)
        {
            var exportForm = new ExportButtonsForm();   
            exportForm.ShowDialog();
        }
    }

    public class UtilityMenuStripItem : ToolStripMenuItem
    {
        public UtilityMenuStripItem()
        {
            this.Text = "Utility";
            this.DropDownItems.Add(new ButtonSetColoursItem());
            this.DropDownItems.Add(new ButtonsShowCubaseCommands());
            this.DropDownItems.Add(new ShowCommandsJsonFile());
            this.DropDownItems.Add(new ShowLogFileItem());  
        }
    }

    public class ShowCommandsJsonFile : ToolStripMenuItem
    {
        public ShowCommandsJsonFile()
        {
            this.Text = "Edit Cubase Commands";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubaseServerSettings = new CubaseServerSettings();
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "notepad.exe";
            p.StartInfo.Arguments = cubaseServerSettings.FilePath;

            p.Start();
        }
    }

    public class ButtonsShowCubaseCommands : ToolStripMenuItem
    {
        public ButtonsShowCubaseCommands()
        {
            this.Text = "Show All Cubase Commands";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubaseCommands = new CubaseCommandsForm();
            cubaseCommands.ShowDialog();    
        }
    }

    public class ButtonSetColoursItem : ToolStripMenuItem
    {
        public ButtonSetColoursItem()
        {
            this.Text = "Set Default Button Colours";
        }

        protected override void OnClick(EventArgs e)
        {
            var colourForm = new SetButtonColoursForm();
            colourForm.ShowDialog();    
        }
    }


    public class FileMenuStripItem : ToolStripMenuItem
    {
        public FileMenuStripItem(Form1 form)
        {
            this.Text = "File";
            this.DropDownItems.Add(new FileExitMenuItem(form));
        }
    }

    public class FileExitMenuItem : ToolStripMenuItem
    {
        private Form1 form;

        public FileExitMenuItem(Form1 form)
        {
            this.form = form;
            this.Text = "Exit";
        }

        protected override void OnClick(EventArgs e)
        {
            this.form.Close();
        }
    }
}
