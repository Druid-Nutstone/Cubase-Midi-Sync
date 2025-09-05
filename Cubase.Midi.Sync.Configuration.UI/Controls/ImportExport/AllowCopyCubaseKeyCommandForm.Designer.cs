namespace Cubase.Midi.Sync.Configuration.UI.Controls.ImportExport
{
    partial class AllowCopyCubaseKeyCommandForm
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
            label1 = new Label();
            CubaseArea = new Label();
            CubaseAreaCopyButton = new Button();
            label2 = new Label();
            CubaseKey = new Label();
            CubaseKeyCopyButton = new Button();
            OKButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(43, 18);
            label1.Name = "label1";
            label1.Size = new Size(84, 17);
            label1.TabIndex = 0;
            label1.Text = "Cubase Area";
            // 
            // CubaseArea
            // 
            CubaseArea.AutoSize = true;
            CubaseArea.Location = new Point(45, 39);
            CubaseArea.Name = "CubaseArea";
            CubaseArea.Size = new Size(216, 17);
            CubaseArea.TabIndex = 1;
            CubaseArea.Text = "Cubase Area Description Goes here";
            // 
            // CubaseAreaCopyButton
            // 
            CubaseAreaCopyButton.Location = new Point(403, 35);
            CubaseAreaCopyButton.Name = "CubaseAreaCopyButton";
            CubaseAreaCopyButton.Size = new Size(83, 25);
            CubaseAreaCopyButton.TabIndex = 2;
            CubaseAreaCopyButton.Text = "Copy";
            CubaseAreaCopyButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(43, 92);
            label2.Name = "label2";
            label2.Size = new Size(311, 17);
            label2.TabIndex = 3;
            label2.Text = "Cubase Key (press the same key in keycommand)";
            // 
            // CubaseKey
            // 
            CubaseKey.AutoSize = true;
            CubaseKey.Location = new Point(47, 111);
            CubaseKey.Name = "CubaseKey";
            CubaseKey.Size = new Size(175, 17);
            CubaseKey.TabIndex = 4;
            CubaseKey.Text = "Cubase Key Value Goes here";
            // 
            // CubaseKeyCopyButton
            // 
            CubaseKeyCopyButton.Location = new Point(403, 107);
            CubaseKeyCopyButton.Name = "CubaseKeyCopyButton";
            CubaseKeyCopyButton.Size = new Size(83, 25);
            CubaseKeyCopyButton.TabIndex = 5;
            CubaseKeyCopyButton.Text = "Copy";
            CubaseKeyCopyButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            OKButton.Location = new Point(49, 189);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(83, 25);
            OKButton.TabIndex = 6;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            // 
            // AllowCopyCubaseKeyCommandForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(540, 247);
            Controls.Add(OKButton);
            Controls.Add(CubaseKeyCopyButton);
            Controls.Add(CubaseKey);
            Controls.Add(label2);
            Controls.Add(CubaseAreaCopyButton);
            Controls.Add(CubaseArea);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "AllowCopyCubaseKeyCommandForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Copy To Cubase To Create The Key Mapping";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label CubaseArea;
        private Button CubaseAreaCopyButton;
        private Label label2;
        private Label CubaseKey;
        private Button CubaseKeyCopyButton;
        private Button OKButton;
    }
}