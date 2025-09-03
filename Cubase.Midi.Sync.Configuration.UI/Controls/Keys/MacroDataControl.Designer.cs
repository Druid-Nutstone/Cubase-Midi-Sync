namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    partial class MacroDataControl
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
            label1 = new Label();
            stringListControl = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.StringListControl();
            ButtonAddCommand = new Button();
            ButtonRemoveCommand = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(18, 11);
            label1.Name = "label1";
            label1.Size = new Size(101, 17);
            label1.TabIndex = 0;
            label1.Text = "Command List ";
            // 
            // stringListControl
            // 
            stringListControl.FormattingEnabled = true;
            stringListControl.Location = new Point(18, 31);
            stringListControl.Name = "stringListControl";
            stringListControl.Size = new Size(131, 106);
            stringListControl.TabIndex = 1;
            // 
            // ButtonAddCommand
            // 
            ButtonAddCommand.Location = new Point(168, 31);
            ButtonAddCommand.Name = "ButtonAddCommand";
            ButtonAddCommand.Size = new Size(125, 25);
            ButtonAddCommand.TabIndex = 2;
            ButtonAddCommand.Text = "Add Command";
            ButtonAddCommand.UseVisualStyleBackColor = true;
            // 
            // ButtonRemoveCommand
            // 
            ButtonRemoveCommand.Location = new Point(168, 62);
            ButtonRemoveCommand.Name = "ButtonRemoveCommand";
            ButtonRemoveCommand.Size = new Size(125, 25);
            ButtonRemoveCommand.TabIndex = 3;
            ButtonRemoveCommand.Text = "Remove Command";
            ButtonRemoveCommand.UseVisualStyleBackColor = true;
            // 
            // MacroDataControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ButtonRemoveCommand);
            Controls.Add(ButtonAddCommand);
            Controls.Add(stringListControl);
            Controls.Add(label1);
            Name = "MacroDataControl";
            Size = new Size(427, 189);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Custom.StringListControl stringListControl;
        private Button ButtonAddCommand;
        private Button ButtonRemoveCommand;
    }
}
