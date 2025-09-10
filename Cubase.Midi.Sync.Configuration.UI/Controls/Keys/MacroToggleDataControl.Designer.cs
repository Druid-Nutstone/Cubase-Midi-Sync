namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    partial class MacroToggleDataControl
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
            ToggleOnCommands = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.StringListControl();
            label2 = new Label();
            ToggleOffCommands = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.StringListControl();
            ToogleOnAddButton = new Button();
            ToggleOffAddButton = new Button();
            RemoveToggleOnButton = new Button();
            RemoveToggleOffButton = new Button();
            ToggleOnInternalCommand = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.InternalCommandButton();
            ToggleOffInternalCommand = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.InternalCommandButton();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(145, 17);
            label1.TabIndex = 0;
            label1.Text = "Toggle On Commands";
            // 
            // ToggleOnCommands
            // 
            ToggleOnCommands.FormattingEnabled = true;
            ToggleOnCommands.Location = new Point(19, 30);
            ToggleOnCommands.Name = "ToggleOnCommands";
            ToggleOnCommands.Size = new Size(120, 106);
            ToggleOnCommands.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(213, 10);
            label2.Name = "label2";
            label2.Size = new Size(147, 17);
            label2.TabIndex = 2;
            label2.Text = "Toggle Off Commands";
            // 
            // ToggleOffCommands
            // 
            ToggleOffCommands.FormattingEnabled = true;
            ToggleOffCommands.Location = new Point(223, 30);
            ToggleOffCommands.Name = "ToggleOffCommands";
            ToggleOffCommands.Size = new Size(137, 106);
            ToggleOffCommands.TabIndex = 3;
            // 
            // ToogleOnAddButton
            // 
            ToogleOnAddButton.Location = new Point(145, 30);
            ToogleOnAddButton.Name = "ToogleOnAddButton";
            ToogleOnAddButton.Size = new Size(59, 25);
            ToogleOnAddButton.TabIndex = 4;
            ToogleOnAddButton.Text = "< Add";
            ToogleOnAddButton.UseVisualStyleBackColor = true;
            // 
            // ToggleOffAddButton
            // 
            ToggleOffAddButton.Location = new Point(366, 30);
            ToggleOffAddButton.Name = "ToggleOffAddButton";
            ToggleOffAddButton.Size = new Size(59, 25);
            ToggleOffAddButton.TabIndex = 5;
            ToggleOffAddButton.Text = "< Add";
            ToggleOffAddButton.UseVisualStyleBackColor = true;
            // 
            // RemoveToggleOnButton
            // 
            RemoveToggleOnButton.Location = new Point(145, 61);
            RemoveToggleOnButton.Name = "RemoveToggleOnButton";
            RemoveToggleOnButton.Size = new Size(59, 25);
            RemoveToggleOnButton.TabIndex = 6;
            RemoveToggleOnButton.Text = "< Del";
            RemoveToggleOnButton.UseVisualStyleBackColor = true;
            // 
            // RemoveToggleOffButton
            // 
            RemoveToggleOffButton.Location = new Point(366, 61);
            RemoveToggleOffButton.Name = "RemoveToggleOffButton";
            RemoveToggleOffButton.Size = new Size(59, 25);
            RemoveToggleOffButton.TabIndex = 7;
            RemoveToggleOffButton.Text = "< Del";
            RemoveToggleOffButton.UseVisualStyleBackColor = true;
            // 
            // ToggleOnInternalCommand
            // 
            ToggleOnInternalCommand.Location = new Point(19, 142);
            ToggleOnInternalCommand.Name = "ToggleOnInternalCommand";
            ToggleOnInternalCommand.Size = new Size(138, 25);
            ToggleOnInternalCommand.TabIndex = 8;
            ToggleOnInternalCommand.Text = "Internal Command";
            ToggleOnInternalCommand.UseVisualStyleBackColor = true;
            // 
            // ToggleOffInternalCommand
            // 
            ToggleOffInternalCommand.Location = new Point(223, 142);
            ToggleOffInternalCommand.Name = "ToggleOffInternalCommand";
            ToggleOffInternalCommand.Size = new Size(137, 25);
            ToggleOffInternalCommand.TabIndex = 9;
            ToggleOffInternalCommand.Text = "Internal Command";
            ToggleOffInternalCommand.UseVisualStyleBackColor = true;
            // 
            // MacroToggleDataControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ToggleOffInternalCommand);
            Controls.Add(ToggleOnInternalCommand);
            Controls.Add(RemoveToggleOffButton);
            Controls.Add(RemoveToggleOnButton);
            Controls.Add(ToggleOffAddButton);
            Controls.Add(ToogleOnAddButton);
            Controls.Add(ToggleOffCommands);
            Controls.Add(label2);
            Controls.Add(ToggleOnCommands);
            Controls.Add(label1);
            Name = "MacroToggleDataControl";
            Size = new Size(446, 200);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Custom.StringListControl ToggleOnCommands;
        private Label label2;
        private Custom.StringListControl ToggleOffCommands;
        private Button ToogleOnAddButton;
        private Button ToggleOffAddButton;
        private Button RemoveToggleOnButton;
        private Button RemoveToggleOffButton;
        private Custom.InternalCommandButton ToggleOnInternalCommand;
        private Custom.InternalCommandButton ToggleOffInternalCommand;
    }
}
