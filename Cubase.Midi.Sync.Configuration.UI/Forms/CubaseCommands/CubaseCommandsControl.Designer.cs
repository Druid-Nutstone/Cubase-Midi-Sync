namespace Cubase.Midi.Sync.Configuration.UI.Forms.CubaseCommands
{
    partial class CubaseCommandsControl
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
            searchFilter = new TextBox();
            label1 = new Label();
            panel2 = new Panel();
            cubaseCommandsListView = new CubaseCommandsListView();
            ShowAssignedCheckBox = new CheckBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(ShowAssignedCheckBox);
            panel1.Controls.Add(searchFilter);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(486, 77);
            panel1.TabIndex = 0;
            // 
            // searchFilter
            // 
            searchFilter.Location = new Point(80, 22);
            searchFilter.Name = "searchFilter";
            searchFilter.Size = new Size(211, 25);
            searchFilter.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(29, 25);
            label1.Name = "label1";
            label1.Size = new Size(45, 17);
            label1.TabIndex = 0;
            label1.Text = "Filter:";
            // 
            // panel2
            // 
            panel2.Controls.Add(cubaseCommandsListView);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 77);
            panel2.Name = "panel2";
            panel2.Size = new Size(486, 273);
            panel2.TabIndex = 1;
            // 
            // cubaseCommandsListView
            // 
            cubaseCommandsListView.Dock = DockStyle.Fill;
            cubaseCommandsListView.FullRowSelect = true;
            cubaseCommandsListView.Location = new Point(0, 0);
            cubaseCommandsListView.MultiSelect = false;
            cubaseCommandsListView.Name = "cubaseCommandsListView";
            cubaseCommandsListView.Size = new Size(486, 273);
            cubaseCommandsListView.TabIndex = 0;
            cubaseCommandsListView.UseCompatibleStateImageBehavior = false;
            cubaseCommandsListView.View = View.Details;
            // 
            // ShowAssignedCheckBox
            // 
            ShowAssignedCheckBox.AutoSize = true;
            ShowAssignedCheckBox.Location = new Point(317, 24);
            ShowAssignedCheckBox.Name = "ShowAssignedCheckBox";
            ShowAssignedCheckBox.Size = new Size(115, 21);
            ShowAssignedCheckBox.TabIndex = 2;
            ShowAssignedCheckBox.Text = "Show Assigned";
            ShowAssignedCheckBox.UseVisualStyleBackColor = true;
            // 
            // CubaseCommandsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "CubaseCommandsControl";
            Size = new Size(486, 350);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private CubaseCommandsListView cubaseCommandsListView;
        private TextBox searchFilter;
        private Label label1;
        private CheckBox ShowAssignedCheckBox;
    }
}
