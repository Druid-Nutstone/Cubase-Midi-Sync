namespace Cubase.Midi.Sync.Configuration.UI.Controls.InternalCommands
{
    partial class NavigateInternalCommandControl
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
            AreaComboBox = new ComboBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(40, 29);
            label1.Name = "label1";
            label1.Size = new Size(133, 17);
            label1.TabIndex = 0;
            label1.Text = "Area To Navigate To";
            // 
            // AreaComboBox
            // 
            AreaComboBox.FormattingEnabled = true;
            AreaComboBox.Location = new Point(44, 50);
            AreaComboBox.Name = "AreaComboBox";
            AreaComboBox.Size = new Size(192, 25);
            AreaComboBox.TabIndex = 1;
            // 
            // NavigateInternalCommandControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(AreaComboBox);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 8.830189F);
            Name = "NavigateInternalCommandControl";
            Size = new Size(402, 204);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox AreaComboBox;
    }
}
