using Cubase.Midi.Sync.Configuration.UI.Forms;
using System;
using System.Collections.Generic;
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
            this.menu.Items.Add(new ButtonMenuStripItem());
            this.menu.Items.Add(new ExportMenuStripItem());
            this.menu.Items.Add(new ImportMenuStripItem());
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

    public class ButtonMenuStripItem : ToolStripMenuItem
    {
        public ButtonMenuStripItem()
        {
            this.Text = "Buttons";
            this.DropDownItems.Add(new ButtonSetColoursItem());
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
