namespace Cubase.Midi.Sync.Configuration.UI.Controls.InternalCommands
{
    partial class InternalCommandForm
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
            CommandComboBox = new ComboBox();
            label1 = new Label();
            CommandPanel = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(CommandComboBox);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(583, 72);
            panel1.TabIndex = 0;
            // 
            // CommandComboBox
            // 
            CommandComboBox.FormattingEnabled = true;
            CommandComboBox.Location = new Point(21, 29);
            CommandComboBox.Name = "CommandComboBox";
            CommandComboBox.Size = new Size(194, 25);
            CommandComboBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(21, 9);
            label1.Name = "label1";
            label1.Size = new Size(115, 17);
            label1.TabIndex = 0;
            label1.Text = "Command Name ";
            // 
            // CommandPanel
            // 
            CommandPanel.Dock = DockStyle.Fill;
            CommandPanel.Location = new Point(0, 72);
            CommandPanel.Name = "CommandPanel";
            CommandPanel.Size = new Size(583, 190);
            CommandPanel.TabIndex = 1;
            // 
            // InternalCommandForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(583, 262);
            Controls.Add(CommandPanel);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "InternalCommandForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Create Internal Command";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private ComboBox CommandComboBox;
        private Panel CommandPanel;
    }
}