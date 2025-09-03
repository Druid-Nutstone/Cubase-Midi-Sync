namespace Cubase.Midi.Sync.Configuration.UI.Controls.Macros
{
    partial class MacroCommandInternalSelectorlForm
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
            commandsMainControl = new Cubase.Midi.Sync.Configuration.UI.Controls.Commands.CommandsMainControl();
            SuspendLayout();
            // 
            // commandsMainControl
            // 
            commandsMainControl.Dock = DockStyle.Fill;
            commandsMainControl.Location = new Point(0, 0);
            commandsMainControl.Name = "commandsMainControl";
            commandsMainControl.Size = new Size(463, 490);
            commandsMainControl.TabIndex = 0;
            // 
            // MacroCommanInternalSelectorlForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(463, 490);
            Controls.Add(commandsMainControl);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "MacroCommanInternalSelectorlForm";
            Text = "Select command from Existing Commands";
            ResumeLayout(false);
        }

        #endregion

        private Commands.CommandsMainControl commandsMainControl;
    }
}