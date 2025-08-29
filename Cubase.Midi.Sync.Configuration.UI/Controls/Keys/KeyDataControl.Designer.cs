namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    partial class KeyDataControl
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
            panel1 = new Panel();
            cbAllocatedOnly = new CheckBox();
            KeyPanel = new Panel();
            keysListView = new KeysListView();
            label1 = new Label();
            categoryLabel = new Label();
            panel1.SuspendLayout();
            KeyPanel.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(categoryLabel);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(cbAllocatedOnly);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(466, 56);
            panel1.TabIndex = 0;
            // 
            // cbAllocatedOnly
            // 
            cbAllocatedOnly.AutoSize = true;
            cbAllocatedOnly.Location = new Point(19, 15);
            cbAllocatedOnly.Name = "cbAllocatedOnly";
            cbAllocatedOnly.Size = new Size(150, 21);
            cbAllocatedOnly.TabIndex = 0;
            cbAllocatedOnly.Text = "Only Show Allocated ";
            cbAllocatedOnly.UseVisualStyleBackColor = true;
            // 
            // KeyPanel
            // 
            KeyPanel.Controls.Add(keysListView);
            KeyPanel.Dock = DockStyle.Fill;
            KeyPanel.Location = new Point(0, 56);
            KeyPanel.Name = "KeyPanel";
            KeyPanel.Size = new Size(466, 204);
            KeyPanel.TabIndex = 1;
            // 
            // keysListView
            // 
            keysListView.Dock = DockStyle.Fill;
            keysListView.Location = new Point(0, 0);
            keysListView.Name = "keysListView";
            keysListView.Size = new Size(466, 204);
            keysListView.TabIndex = 0;
            keysListView.UseCompatibleStateImageBehavior = false;
            keysListView.View = View.Details;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(184, 16);
            label1.Name = "label1";
            label1.Size = new Size(64, 17);
            label1.TabIndex = 1;
            label1.Text = "Category:";
            // 
            // categoryLabel
            // 
            categoryLabel.AutoSize = true;
            categoryLabel.Location = new Point(254, 16);
            categoryLabel.Name = "categoryLabel";
            categoryLabel.Size = new Size(0, 17);
            categoryLabel.TabIndex = 2;
            // 
            // KeyDataControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(KeyPanel);
            Controls.Add(panel1);
            Name = "KeyDataControl";
            Size = new Size(466, 260);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            KeyPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel KeyPanel;
        private KeysListView keysListView;
        private CheckBox cbAllocatedOnly;
        private Label categoryLabel;
        private Label label1;
    }
}
