namespace Cubase.Midi.Sync.Configuration.UI.Controls.Mapping
{
    partial class MappingMainControl
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
            LeftPanel = new Panel();
            commandsListView = new Cubase.Midi.Sync.Configuration.UI.Controls.Commands.CommandsListView();
            ButtonPanel = new Panel();
            ButtonCopy = new Button();
            RightPanel = new Panel();
            mappingListView = new MappingListView();
            panel2 = new Panel();
            ExistingArea = new ComboBox();
            label2 = new Label();
            AreaTextColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            AreaBackgroundColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            CreateButton = new Button();
            NewAreaName = new TextBox();
            label1 = new Label();
            LeftPanel.SuspendLayout();
            ButtonPanel.SuspendLayout();
            RightPanel.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // LeftPanel
            // 
            LeftPanel.Controls.Add(commandsListView);
            LeftPanel.Dock = DockStyle.Left;
            LeftPanel.Location = new Point(0, 0);
            LeftPanel.Name = "LeftPanel";
            LeftPanel.Size = new Size(260, 387);
            LeftPanel.TabIndex = 0;
            // 
            // commandsListView
            // 
            commandsListView.AllowDrop = true;
            commandsListView.Dock = DockStyle.Fill;
            commandsListView.FullRowSelect = true;
            commandsListView.Location = new Point(0, 0);
            commandsListView.MultiSelect = false;
            commandsListView.Name = "commandsListView";
            commandsListView.Size = new Size(260, 387);
            commandsListView.TabIndex = 0;
            commandsListView.UseCompatibleStateImageBehavior = false;
            commandsListView.View = View.Details;
            // 
            // ButtonPanel
            // 
            ButtonPanel.BackColor = Color.WhiteSmoke;
            ButtonPanel.BorderStyle = BorderStyle.FixedSingle;
            ButtonPanel.Controls.Add(ButtonCopy);
            ButtonPanel.Dock = DockStyle.Left;
            ButtonPanel.Location = new Point(260, 0);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Size = new Size(106, 387);
            ButtonPanel.TabIndex = 1;
            // 
            // ButtonCopy
            // 
            ButtonCopy.Location = new Point(16, 175);
            ButtonCopy.Name = "ButtonCopy";
            ButtonCopy.Size = new Size(70, 25);
            ButtonCopy.TabIndex = 0;
            ButtonCopy.Text = "Copy >>";
            ButtonCopy.UseVisualStyleBackColor = true;
            // 
            // RightPanel
            // 
            RightPanel.BackColor = SystemColors.Window;
            RightPanel.Controls.Add(mappingListView);
            RightPanel.Controls.Add(panel2);
            RightPanel.Dock = DockStyle.Fill;
            RightPanel.Location = new Point(366, 0);
            RightPanel.Name = "RightPanel";
            RightPanel.Size = new Size(577, 387);
            RightPanel.TabIndex = 2;
            // 
            // mappingListView
            // 
            mappingListView.AllowDrop = true;
            mappingListView.Dock = DockStyle.Fill;
            mappingListView.Enabled = false;
            mappingListView.FullRowSelect = true;
            mappingListView.Location = new Point(0, 136);
            mappingListView.Name = "mappingListView";
            mappingListView.Size = new Size(577, 251);
            mappingListView.TabIndex = 1;
            mappingListView.UseCompatibleStateImageBehavior = false;
            mappingListView.View = View.Details;
            // 
            // panel2
            // 
            panel2.Controls.Add(ExistingArea);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(AreaTextColour);
            panel2.Controls.Add(AreaBackgroundColour);
            panel2.Controls.Add(CreateButton);
            panel2.Controls.Add(NewAreaName);
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(577, 136);
            panel2.TabIndex = 0;
            // 
            // ExistingArea
            // 
            ExistingArea.FormattingEnabled = true;
            ExistingArea.Location = new Point(313, 28);
            ExistingArea.Name = "ExistingArea";
            ExistingArea.Size = new Size(168, 25);
            ExistingArea.TabIndex = 11;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(313, 7);
            label2.Name = "label2";
            label2.Size = new Size(115, 17);
            label2.TabIndex = 10;
            label2.Text = "Use Existing Area";
            // 
            // AreaTextColour
            // 
            AreaTextColour.Label = "Area Button Text Colour";
            AreaTextColour.Location = new Point(313, 69);
            AreaTextColour.Name = "AreaTextColour";
            AreaTextColour.Size = new Size(259, 61);
            AreaTextColour.TabIndex = 9;
            // 
            // AreaBackgroundColour
            // 
            AreaBackgroundColour.Label = "Area Button Background Colour";
            AreaBackgroundColour.Location = new Point(18, 68);
            AreaBackgroundColour.Name = "AreaBackgroundColour";
            AreaBackgroundColour.Size = new Size(259, 61);
            AreaBackgroundColour.TabIndex = 8;
            // 
            // CreateButton
            // 
            CreateButton.Location = new Point(196, 27);
            CreateButton.Name = "CreateButton";
            CreateButton.Size = new Size(83, 25);
            CreateButton.TabIndex = 7;
            CreateButton.Text = "Create";
            CreateButton.UseVisualStyleBackColor = true;
            // 
            // NewAreaName
            // 
            NewAreaName.Location = new Point(18, 27);
            NewAreaName.Name = "NewAreaName";
            NewAreaName.Size = new Size(172, 25);
            NewAreaName.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(18, 7);
            label1.Name = "label1";
            label1.Size = new Size(107, 17);
            label1.TabIndex = 5;
            label1.Text = "New Area Name";
            // 
            // MappingMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            Controls.Add(RightPanel);
            Controls.Add(ButtonPanel);
            Controls.Add(LeftPanel);
            Name = "MappingMainControl";
            Size = new Size(943, 387);
            LeftPanel.ResumeLayout(false);
            ButtonPanel.ResumeLayout(false);
            RightPanel.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel LeftPanel;
        private Commands.CommandsListView commandsListView;
        private Panel ButtonPanel;
        private Panel RightPanel;
        private MappingListView mappingListView;
        private Panel panel2;
        private Custom.ColourPickerControl AreaTextColour;
        private Custom.ColourPickerControl AreaBackgroundColour;
        private Button CreateButton;
        private TextBox NewAreaName;
        private Label label1;
        private Button ButtonCopy;
        private ComboBox ExistingArea;
        private Label label2;
    }
}
