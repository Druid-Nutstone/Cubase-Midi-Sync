namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas.RenameForm
{
    partial class RenameCommandForm
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
            ButtonOK = new Button();
            label1 = new Label();
            CurrentNameText = new TextBox();
            label2 = new Label();
            NewNameText = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(ButtonOK);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 122);
            panel1.Name = "panel1";
            panel1.Size = new Size(590, 51);
            panel1.TabIndex = 0;
            // 
            // ButtonOK
            // 
            ButtonOK.Location = new Point(23, 14);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new Size(83, 25);
            ButtonOK.TabIndex = 0;
            ButtonOK.Text = "OK";
            ButtonOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(34, 23);
            label1.Name = "label1";
            label1.Size = new Size(94, 17);
            label1.TabIndex = 1;
            label1.Text = "Current Name";
            // 
            // CurrentNameText
            // 
            CurrentNameText.Enabled = false;
            CurrentNameText.Location = new Point(34, 43);
            CurrentNameText.Name = "CurrentNameText";
            CurrentNameText.Size = new Size(224, 25);
            CurrentNameText.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(301, 23);
            label2.Name = "label2";
            label2.Size = new Size(75, 17);
            label2.TabIndex = 3;
            label2.Text = "New Name";
            // 
            // NewNameText
            // 
            NewNameText.Location = new Point(301, 43);
            NewNameText.Name = "NewNameText";
            NewNameText.Size = new Size(246, 25);
            NewNameText.TabIndex = 4;
            // 
            // RenameCommandForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(590, 173);
            Controls.Add(NewNameText);
            Controls.Add(label2);
            Controls.Add(CurrentNameText);
            Controls.Add(label1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "RenameCommandForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rename Command";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Button ButtonOK;
        private Label label1;
        private TextBox CurrentNameText;
        private Label label2;
        private TextBox NewNameText;
    }
}