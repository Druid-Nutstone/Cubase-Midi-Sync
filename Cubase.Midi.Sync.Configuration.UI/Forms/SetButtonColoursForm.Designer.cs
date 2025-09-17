namespace Cubase.Midi.Sync.Configuration.UI.Forms
{
    partial class SetButtonColoursForm
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
            BackgroundColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            TextColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            ButtonExampleControl = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ButtonExampleControl();
            ButtonOK = new Button();
            UseDefaultsCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // BackgroundColour
            // 
            BackgroundColour.Label = "Background Colour";
            BackgroundColour.Location = new Point(43, 33);
            BackgroundColour.Name = "BackgroundColour";
            BackgroundColour.Size = new Size(259, 61);
            BackgroundColour.TabIndex = 0;
            // 
            // TextColour
            // 
            TextColour.Label = "User Assigned";
            TextColour.Location = new Point(319, 33);
            TextColour.Name = "TextColour";
            TextColour.Size = new Size(259, 61);
            TextColour.TabIndex = 1;
            // 
            // ButtonExampleControl
            // 
            ButtonExampleControl.Location = new Point(604, 49);
            ButtonExampleControl.Name = "ButtonExampleControl";
            ButtonExampleControl.Size = new Size(163, 45);
            ButtonExampleControl.TabIndex = 2;
            // 
            // ButtonOK
            // 
            ButtonOK.Location = new Point(48, 130);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new Size(83, 25);
            ButtonOK.TabIndex = 3;
            ButtonOK.Text = "OK";
            ButtonOK.UseVisualStyleBackColor = true;
            // 
            // UseDefaultsCheckBox
            // 
            UseDefaultsCheckBox.AutoSize = true;
            UseDefaultsCheckBox.Location = new Point(604, 130);
            UseDefaultsCheckBox.Name = "UseDefaultsCheckBox";
            UseDefaultsCheckBox.Size = new Size(100, 21);
            UseDefaultsCheckBox.TabIndex = 4;
            UseDefaultsCheckBox.Text = "Use Defaults";
            UseDefaultsCheckBox.UseVisualStyleBackColor = true;
            // 
            // SetButtonColoursForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(800, 195);
            Controls.Add(UseDefaultsCheckBox);
            Controls.Add(ButtonOK);
            Controls.Add(ButtonExampleControl);
            Controls.Add(TextColour);
            Controls.Add(BackgroundColour);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "SetButtonColoursForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Reset Button Colours ";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Controls.Custom.ColourPickerControl BackgroundColour;
        private Controls.Custom.ColourPickerControl TextColour;
        private Controls.Custom.ButtonExampleControl ButtonExampleControl;
        private Button ButtonOK;
        private CheckBox UseDefaultsCheckBox;
    }
}