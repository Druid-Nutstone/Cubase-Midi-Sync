namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros.Forms
{
    partial class AddMidiMacroForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            midiMacroSelectListView = new MidiMacroSelectListView();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(501, 76);
            panel1.TabIndex = 0;
            // 
            // midiMacroSelectListView
            // 
            midiMacroSelectListView.Dock = DockStyle.Fill;
            midiMacroSelectListView.FullRowSelect = true;
            midiMacroSelectListView.Location = new Point(0, 76);
            midiMacroSelectListView.MultiSelect = false;
            midiMacroSelectListView.Name = "midiMacroSelectListView";
            midiMacroSelectListView.Size = new Size(501, 374);
            midiMacroSelectListView.TabIndex = 1;
            midiMacroSelectListView.UseCompatibleStateImageBehavior = false;
            midiMacroSelectListView.View = View.Details;
            // 
            // AddMidiMacroForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(501, 450);
            Controls.Add(midiMacroSelectListView);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "AddMidiMacroForm";
            StartPosition = FormStartPosition.Manual;
            Text = "Add Macro ";
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private MidiMacroSelectListView midiMacroSelectListView;
    }
}