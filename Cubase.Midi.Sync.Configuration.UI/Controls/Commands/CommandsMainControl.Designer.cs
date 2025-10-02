namespace Cubase.Midi.Sync.Configuration.UI.Controls.Commands
{
    partial class CommandsMainControl
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
            AddMidiCommandButton = new Button();
            AddNewButton = new Button();
            AddKeyCommandButton = new Button();
            ClearFilter = new Button();
            label1 = new Label();
            FilterBox = new ComboBox();
            dataPanel = new Panel();
            commandsListView = new CommandsListView();
            AddCommandButton = new Button();
            panel1.SuspendLayout();
            dataPanel.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(AddCommandButton);
            panel1.Controls.Add(AddMidiCommandButton);
            panel1.Controls.Add(AddNewButton);
            panel1.Controls.Add(AddKeyCommandButton);
            panel1.Controls.Add(ClearFilter);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(FilterBox);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(979, 70);
            panel1.TabIndex = 0;
            // 
            // AddMidiCommandButton
            // 
            AddMidiCommandButton.Location = new Point(584, 23);
            AddMidiCommandButton.Name = "AddMidiCommandButton";
            AddMidiCommandButton.Size = new Size(90, 25);
            AddMidiCommandButton.TabIndex = 5;
            AddMidiCommandButton.Text = "Add Midi";
            AddMidiCommandButton.UseVisualStyleBackColor = true;
            // 
            // AddNewButton
            // 
            AddNewButton.Location = new Point(702, 22);
            AddNewButton.Name = "AddNewButton";
            AddNewButton.Size = new Size(104, 25);
            AddNewButton.TabIndex = 4;
            AddNewButton.Text = "Add New ";
            AddNewButton.UseVisualStyleBackColor = true;
            // 
            // AddKeyCommandButton
            // 
            AddKeyCommandButton.Location = new Point(467, 23);
            AddKeyCommandButton.Name = "AddKeyCommandButton";
            AddKeyCommandButton.Size = new Size(84, 25);
            AddKeyCommandButton.TabIndex = 3;
            AddKeyCommandButton.Text = "Add Key";
            AddKeyCommandButton.UseVisualStyleBackColor = true;
            // 
            // ClearFilter
            // 
            ClearFilter.BackColor = SystemColors.Window;
            ClearFilter.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            ClearFilter.Location = new Point(265, 23);
            ClearFilter.Name = "ClearFilter";
            ClearFilter.Size = new Size(27, 25);
            ClearFilter.TabIndex = 2;
            ClearFilter.Text = "X";
            ClearFilter.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(18, 26);
            label1.Name = "label1";
            label1.Size = new Size(63, 17);
            label1.TabIndex = 1;
            label1.Text = "Filter By:";
            // 
            // FilterBox
            // 
            FilterBox.FormattingEnabled = true;
            FilterBox.Location = new Point(87, 23);
            FilterBox.Name = "FilterBox";
            FilterBox.Size = new Size(172, 25);
            FilterBox.TabIndex = 0;
            // 
            // dataPanel
            // 
            dataPanel.Controls.Add(commandsListView);
            dataPanel.Dock = DockStyle.Fill;
            dataPanel.Location = new Point(0, 70);
            dataPanel.Name = "dataPanel";
            dataPanel.Size = new Size(979, 260);
            dataPanel.TabIndex = 1;
            // 
            // commandsListView
            // 
            commandsListView.Dock = DockStyle.Fill;
            commandsListView.FullRowSelect = true;
            commandsListView.Location = new Point(0, 0);
            commandsListView.MultiSelect = false;
            commandsListView.Name = "commandsListView";
            commandsListView.Size = new Size(979, 260);
            commandsListView.TabIndex = 0;
            commandsListView.UseCompatibleStateImageBehavior = false;
            commandsListView.View = View.Details;
            // 
            // AddCommandButton
            // 
            AddCommandButton.Location = new Point(320, 22);
            AddCommandButton.Name = "AddCommandButton";
            AddCommandButton.Size = new Size(114, 25);
            AddCommandButton.TabIndex = 6;
            AddCommandButton.Text = "Add Command";
            AddCommandButton.UseVisualStyleBackColor = true;
            // 
            // CommandsMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dataPanel);
            Controls.Add(panel1);
            Name = "CommandsMainControl";
            Size = new Size(979, 330);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            dataPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel dataPanel;
        private CommandsListView commandsListView;
        private Label label1;
        private ComboBox FilterBox;
        private Button ClearFilter;
        private Button AddKeyCommandButton;
        private Button AddNewButton;
        private Button AddMidiCommandButton;
        private Button AddCommandButton;
    }
}
