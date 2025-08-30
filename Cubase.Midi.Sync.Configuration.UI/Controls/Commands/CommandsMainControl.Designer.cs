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
            dataPanel = new Panel();
            commandsListView = new CommandsListView();
            dataPanel.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(536, 70);
            panel1.TabIndex = 0;
            // 
            // dataPanel
            // 
            dataPanel.Controls.Add(commandsListView);
            dataPanel.Dock = DockStyle.Fill;
            dataPanel.Location = new Point(0, 70);
            dataPanel.Name = "dataPanel";
            dataPanel.Size = new Size(536, 260);
            dataPanel.TabIndex = 1;
            // 
            // commandsListView
            // 
            commandsListView.Dock = DockStyle.Fill;
            commandsListView.Location = new Point(0, 0);
            commandsListView.Name = "commandsListView";
            commandsListView.Size = new Size(536, 260);
            commandsListView.TabIndex = 0;
            commandsListView.UseCompatibleStateImageBehavior = false;
            commandsListView.View = View.Details;
            // 
            // CommandsMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dataPanel);
            Controls.Add(panel1);
            Name = "CommandsMainControl";
            Size = new Size(536, 330);
            dataPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel dataPanel;
        private CommandsListView commandsListView;
    }
}
