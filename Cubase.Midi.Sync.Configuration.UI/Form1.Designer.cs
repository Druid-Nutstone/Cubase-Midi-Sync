namespace Cubase.Midi.Sync.Configuration.UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            statusStrip1 = new StatusStrip();
            DataPanel = new Panel();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(18, 18);
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1097, 25);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(18, 18);
            statusStrip1.Location = new Point(0, 475);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1097, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // DataPanel
            // 
            DataPanel.Dock = DockStyle.Fill;
            DataPanel.Location = new Point(0, 25);
            DataPanel.Name = "DataPanel";
            DataPanel.Size = new Size(1097, 450);
            DataPanel.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1097, 497);
            Controls.Add(DataPanel);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cubase Commands";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private StatusStrip statusStrip1;
        private Panel DataPanel;
    }
}
