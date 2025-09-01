namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    partial class ButtonExampleControl
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
            ExampleButton = new Button();
            SuspendLayout();
            // 
            // ExampleButton
            // 
            ExampleButton.Dock = DockStyle.Fill;
            ExampleButton.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            ExampleButton.Location = new Point(0, 0);
            ExampleButton.Name = "ExampleButton";
            ExampleButton.Size = new Size(148, 41);
            ExampleButton.TabIndex = 0;
            ExampleButton.Text = "Not Set";
            ExampleButton.UseVisualStyleBackColor = true;
            // 
            // ButtonExampleControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ExampleButton);
            Name = "ButtonExampleControl";
            Size = new Size(148, 41);
            ResumeLayout(false);
        }

        #endregion

        private Button ExampleButton;
    }
}
