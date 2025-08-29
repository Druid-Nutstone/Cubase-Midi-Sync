namespace Cubase.Midi.Sync.Configuration.UI.Controls
{
    partial class KeysMainControl
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeysMainControl));
            toolTip1 = new ToolTip(components);
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            searchInput = new ToolStripTextBox();
            toolStripSeparator1 = new ToolStripSeparator();
            ListViewPanel = new Panel();
            splitter1 = new Splitter();
            DataPanel = new Panel();
            searchButton = new ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new Size(18, 18);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, searchInput, toolStripSeparator1, searchButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(683, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(50, 22);
            toolStripLabel1.Text = "Search:";
            // 
            // searchInput
            // 
            searchInput.Name = "searchInput";
            searchInput.Size = new Size(255, 25);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // ListViewPanel
            // 
            ListViewPanel.Dock = DockStyle.Left;
            ListViewPanel.Location = new Point(0, 25);
            ListViewPanel.Name = "ListViewPanel";
            ListViewPanel.Size = new Size(221, 355);
            ListViewPanel.TabIndex = 1;
            // 
            // splitter1
            // 
            splitter1.Location = new Point(221, 25);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(3, 355);
            splitter1.TabIndex = 2;
            splitter1.TabStop = false;
            // 
            // DataPanel
            // 
            DataPanel.Dock = DockStyle.Fill;
            DataPanel.Location = new Point(224, 25);
            DataPanel.Name = "DataPanel";
            DataPanel.Size = new Size(459, 355);
            DataPanel.TabIndex = 3;
            // 
            // searchButton
            // 
            searchButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            searchButton.Image = (Image)resources.GetObject("searchButton.Image");
            searchButton.ImageTransparentColor = Color.Magenta;
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(51, 22);
            searchButton.Text = "Search";
            // 
            // KeysMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(DataPanel);
            Controls.Add(splitter1);
            Controls.Add(ListViewPanel);
            Controls.Add(toolStrip1);
            Name = "KeysMainControl";
            Size = new Size(683, 380);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolTip toolTip1;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private Panel ListViewPanel;
        private Splitter splitter1;
        private Panel DataPanel;
        private ToolStripTextBox searchInput;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton searchButton;
    }
}
