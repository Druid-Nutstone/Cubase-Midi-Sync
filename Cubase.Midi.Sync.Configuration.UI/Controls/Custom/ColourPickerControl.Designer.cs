namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    partial class ColourPickerControl
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
            ColourLabel = new Label();
            ColourValue = new TextBox();
            ColourPickerButton = new Button();
            SuspendLayout();
            // 
            // ColourLabel
            // 
            ColourLabel.AutoSize = true;
            ColourLabel.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            ColourLabel.Location = new Point(3, 0);
            ColourLabel.Name = "ColourLabel";
            ColourLabel.Size = new Size(95, 17);
            ColourLabel.TabIndex = 0;
            ColourLabel.Text = "User Assigned";
            // 
            // ColourValue
            // 
            ColourValue.Location = new Point(3, 20);
            ColourValue.Name = "ColourValue";
            ColourValue.Size = new Size(168, 25);
            ColourValue.TabIndex = 1;
            // 
            // ColourPickerButton
            // 
            ColourPickerButton.Location = new Point(177, 20);
            ColourPickerButton.Name = "ColourPickerButton";
            ColourPickerButton.Size = new Size(51, 25);
            ColourPickerButton.TabIndex = 2;
            ColourPickerButton.Text = "Pick";
            ColourPickerButton.UseVisualStyleBackColor = true;
            // 
            // ColourPickerControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ColourPickerButton);
            Controls.Add(ColourValue);
            Controls.Add(ColourLabel);
            Name = "ColourPickerControl";
            Size = new Size(235, 55);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ColourLabel;
        private TextBox ColourValue;
        private Button ColourPickerButton;
    }
}
