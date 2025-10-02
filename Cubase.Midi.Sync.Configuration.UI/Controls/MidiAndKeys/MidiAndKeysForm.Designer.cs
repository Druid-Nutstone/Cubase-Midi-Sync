namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys
{
    partial class MidiAndKeysForm
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
            midiAndKeysControl = new MidiAndKeysControl();
            SuspendLayout();
            // 
            // midiAndKeysControl
            // 
            midiAndKeysControl.Dock = DockStyle.Fill;
            midiAndKeysControl.Location = new Point(0, 0);
            midiAndKeysControl.Name = "midiAndKeysControl";
            midiAndKeysControl.Size = new Size(691, 450);
            midiAndKeysControl.TabIndex = 0;
            // 
            // MidiAndKeysForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(691, 450);
            Controls.Add(midiAndKeysControl);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "MidiAndKeysForm";
            StartPosition = FormStartPosition.Manual;
            Text = "All Defined Commands";
            ResumeLayout(false);
        }

        #endregion

        private MidiAndKeysControl midiAndKeysControl;
    }
}