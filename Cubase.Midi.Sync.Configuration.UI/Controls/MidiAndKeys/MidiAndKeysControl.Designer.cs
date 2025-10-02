namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys
{
    partial class MidiAndKeysControl
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
            ClearSearchButton = new Button();
            SearchText = new TextBox();
            label2 = new Label();
            ClearTypeButton = new Button();
            SelectTypeCombo = new ComboBox();
            label1 = new Label();
            midiAndKeysListView = new MidiAndKeysListView();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(ClearSearchButton);
            panel1.Controls.Add(SearchText);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(ClearTypeButton);
            panel1.Controls.Add(SelectTypeCombo);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(644, 78);
            panel1.TabIndex = 0;
            // 
            // ClearSearchButton
            // 
            ClearSearchButton.Location = new Point(436, 31);
            ClearSearchButton.Name = "ClearSearchButton";
            ClearSearchButton.Size = new Size(21, 25);
            ClearSearchButton.TabIndex = 5;
            ClearSearchButton.Text = "X";
            ClearSearchButton.UseVisualStyleBackColor = true;
            // 
            // SearchText
            // 
            SearchText.Location = new Point(220, 32);
            SearchText.Name = "SearchText";
            SearchText.Size = new Size(210, 25);
            SearchText.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(220, 12);
            label2.Name = "label2";
            label2.Size = new Size(48, 17);
            label2.TabIndex = 3;
            label2.Text = "Search";
            // 
            // ClearTypeButton
            // 
            ClearTypeButton.Location = new Point(166, 31);
            ClearTypeButton.Name = "ClearTypeButton";
            ClearTypeButton.Size = new Size(21, 25);
            ClearTypeButton.TabIndex = 2;
            ClearTypeButton.Text = "X";
            ClearTypeButton.UseVisualStyleBackColor = true;
            // 
            // SelectTypeCombo
            // 
            SelectTypeCombo.FormattingEnabled = true;
            SelectTypeCombo.Location = new Point(26, 32);
            SelectTypeCombo.Name = "SelectTypeCombo";
            SelectTypeCombo.Size = new Size(134, 25);
            SelectTypeCombo.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(26, 12);
            label1.Name = "label1";
            label1.Size = new Size(37, 17);
            label1.TabIndex = 0;
            label1.Text = "Type";
            // 
            // midiAndKeysListView
            // 
            midiAndKeysListView.Dock = DockStyle.Fill;
            midiAndKeysListView.FullRowSelect = true;
            midiAndKeysListView.Location = new Point(0, 78);
            midiAndKeysListView.MultiSelect = false;
            midiAndKeysListView.Name = "midiAndKeysListView";
            midiAndKeysListView.Size = new Size(644, 327);
            midiAndKeysListView.TabIndex = 1;
            midiAndKeysListView.UseCompatibleStateImageBehavior = false;
            midiAndKeysListView.View = View.Details;
            // 
            // MidiAndKeysControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(midiAndKeysListView);
            Controls.Add(panel1);
            Name = "MidiAndKeysControl";
            Size = new Size(644, 405);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private MidiAndKeysListView midiAndKeysListView;
        private Label label1;
        private ComboBox SelectTypeCombo;
        private Button ClearTypeButton;
        private Label label2;
        private Button ClearSearchButton;
        private TextBox SearchText;
    }
}
