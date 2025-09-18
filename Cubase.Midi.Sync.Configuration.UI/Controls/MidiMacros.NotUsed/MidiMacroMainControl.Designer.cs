namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros
{
    partial class MidiMacroMainControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            AddMacroButton = new Button();
            midiMacroListView = new MidiMacroListView();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(AddMacroButton);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(645, 76);
            panel1.TabIndex = 0;
            // 
            // AddMacroButton
            // 
            AddMacroButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AddMacroButton.Location = new Point(531, 25);
            AddMacroButton.Name = "AddMacroButton";
            AddMacroButton.Size = new Size(83, 25);
            AddMacroButton.TabIndex = 0;
            AddMacroButton.Text = "Add Macro";
            AddMacroButton.UseVisualStyleBackColor = true;
            // 
            // midiMacroListView
            // 
            midiMacroListView.Dock = DockStyle.Fill;
            midiMacroListView.FullRowSelect = true;
            midiMacroListView.Location = new Point(0, 76);
            midiMacroListView.MultiSelect = false;
            midiMacroListView.Name = "midiMacroListView";
            midiMacroListView.Size = new Size(645, 236);
            midiMacroListView.TabIndex = 1;
            midiMacroListView.UseCompatibleStateImageBehavior = false;
            midiMacroListView.View = View.Details;
            // 
            // MidiMacroMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(midiMacroListView);
            Controls.Add(panel1);
            Name = "MidiMacroMainControl";
            Size = new Size(645, 312);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private MidiMacroListView midiMacroListView;
        private Button AddMacroButton;
    }
}
