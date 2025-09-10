namespace Cubase.Midi.Sync.Configuration.UI.Controls.Midi
{
    partial class MidiCommandSelectorForm
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
            panel2 = new Panel();
            midiCommandListView = new MidiCommandListView();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(553, 72);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(midiCommandListView);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 72);
            panel2.Name = "panel2";
            panel2.Size = new Size(553, 439);
            panel2.TabIndex = 1;
            // 
            // midiCommandListView
            // 
            midiCommandListView.Dock = DockStyle.Fill;
            midiCommandListView.FullRowSelect = true;
            midiCommandListView.Location = new Point(0, 0);
            midiCommandListView.MultiSelect = false;
            midiCommandListView.Name = "midiCommandListView";
            midiCommandListView.Size = new Size(553, 439);
            midiCommandListView.TabIndex = 0;
            midiCommandListView.UseCompatibleStateImageBehavior = false;
            midiCommandListView.View = View.Details;
            // 
            // MidiCommandSelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(553, 511);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "MidiCommandSelectorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Midi Command";
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private MidiCommandListView midiCommandListView;
    }
}