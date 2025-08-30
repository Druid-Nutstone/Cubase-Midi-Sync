namespace Cubase.Midi.Sync.Configuration.UI.Controls.Macros
{
    partial class MacroCommandSelectorForm
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
            searchFilter = new TextBox();
            label1 = new Label();
            dataPanel = new Panel();
            commandSelectorListView = new CommandSelectorListView();
            panel1.SuspendLayout();
            dataPanel.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(searchFilter);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(633, 55);
            panel1.TabIndex = 0;
            // 
            // searchFilter
            // 
            searchFilter.Location = new Point(68, 16);
            searchFilter.Name = "searchFilter";
            searchFilter.Size = new Size(188, 25);
            searchFilter.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 19);
            label1.Name = "label1";
            label1.Size = new Size(39, 17);
            label1.TabIndex = 0;
            label1.Text = "Filter:";
            // 
            // dataPanel
            // 
            dataPanel.Controls.Add(commandSelectorListView);
            dataPanel.Dock = DockStyle.Fill;
            dataPanel.Location = new Point(0, 55);
            dataPanel.Name = "dataPanel";
            dataPanel.Size = new Size(633, 404);
            dataPanel.TabIndex = 1;
            // 
            // commandSelectorListView
            // 
            commandSelectorListView.Dock = DockStyle.Fill;
            commandSelectorListView.Location = new Point(0, 0);
            commandSelectorListView.MultiSelect = false;
            commandSelectorListView.Name = "commandSelectorListView";
            commandSelectorListView.Size = new Size(633, 404);
            commandSelectorListView.TabIndex = 0;
            commandSelectorListView.UseCompatibleStateImageBehavior = false;
            commandSelectorListView.View = View.Details;
            // 
            // MacroCommandSelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(633, 459);
            Controls.Add(dataPanel);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "MacroCommandSelectorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cubase Command Selector";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            dataPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel dataPanel;
        private CommandSelectorListView commandSelectorListView;
        private TextBox searchFilter;
        private Label label1;
    }
}