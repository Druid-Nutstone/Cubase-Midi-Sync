namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    partial class AddKeyToCommandsForm
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
            label1 = new Label();
            buttonName = new TextBox();
            label2 = new Label();
            action = new TextBox();
            buttonAdd = new Button();
            label5 = new Label();
            cbAreaName = new ComboBox();
            label6 = new Label();
            newAreaName = new TextBox();
            label9 = new Label();
            cbButtonType = new ComboBox();
            buttonCubaseCommands = new Button();
            AreaBackgroundColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            AreaButtonTextColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            ButtonToggleTextColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            ButtonToggleBackgroundColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            ButtonTextColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            ButtonBackgroundColour = new Cubase.Midi.Sync.Configuration.UI.Controls.Custom.ColourPickerControl();
            CopyColourFromArea = new LinkLabel();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(24, 80);
            label1.Name = "label1";
            label1.Size = new Size(183, 17);
            label1.TabIndex = 0;
            label1.Text = "Description (Button Header)";
            // 
            // buttonName
            // 
            buttonName.Location = new Point(24, 100);
            buttonName.Name = "buttonName";
            buttonName.Size = new Size(221, 25);
            buttonName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(276, 80);
            label2.Name = "label2";
            label2.Size = new Size(74, 17);
            label2.TabIndex = 2;
            label2.Text = "Key Action";
            // 
            // action
            // 
            action.Location = new Point(276, 100);
            action.Name = "action";
            action.Size = new Size(217, 25);
            action.TabIndex = 3;
            // 
            // buttonAdd
            // 
            buttonAdd.Location = new Point(21, 527);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(83, 25);
            buttonAdd.TabIndex = 4;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label5.Location = new Point(24, 21);
            label5.Name = "label5";
            label5.Size = new Size(76, 17);
            label5.TabIndex = 12;
            label5.Text = "Area Name";
            // 
            // cbAreaName
            // 
            cbAreaName.FormattingEnabled = true;
            cbAreaName.Location = new Point(24, 41);
            cbAreaName.Name = "cbAreaName";
            cbAreaName.Size = new Size(221, 25);
            cbAreaName.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label6.Location = new Point(276, 21);
            label6.Name = "label6";
            label6.Size = new Size(71, 17);
            label6.TabIndex = 14;
            label6.Text = "New Area ";
            // 
            // newAreaName
            // 
            newAreaName.Location = new Point(276, 41);
            newAreaName.Name = "newAreaName";
            newAreaName.Size = new Size(217, 25);
            newAreaName.TabIndex = 15;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label9.Location = new Point(24, 21);
            label9.Name = "label9";
            label9.Size = new Size(83, 17);
            label9.TabIndex = 22;
            label9.Text = "Button Type";
            // 
            // cbButtonType
            // 
            cbButtonType.FormattingEnabled = true;
            cbButtonType.Location = new Point(24, 38);
            cbButtonType.Name = "cbButtonType";
            cbButtonType.Size = new Size(158, 25);
            cbButtonType.TabIndex = 23;
            // 
            // buttonCubaseCommands
            // 
            buttonCubaseCommands.Location = new Point(513, 99);
            buttonCubaseCommands.Name = "buttonCubaseCommands";
            buttonCubaseCommands.Size = new Size(156, 25);
            buttonCubaseCommands.TabIndex = 24;
            buttonCubaseCommands.Text = "Cubase Commands >>";
            buttonCubaseCommands.UseVisualStyleBackColor = true;
            // 
            // AreaBackgroundColour
            // 
            AreaBackgroundColour.Label = "Area Background Button Colour";
            AreaBackgroundColour.Location = new Point(24, 80);
            AreaBackgroundColour.Name = "AreaBackgroundColour";
            AreaBackgroundColour.Size = new Size(256, 70);
            AreaBackgroundColour.TabIndex = 25;
            // 
            // AreaButtonTextColour
            // 
            AreaButtonTextColour.Label = "Area Button Text Colour";
            AreaButtonTextColour.Location = new Point(276, 80);
            AreaButtonTextColour.Name = "AreaButtonTextColour";
            AreaButtonTextColour.Size = new Size(259, 64);
            AreaButtonTextColour.TabIndex = 26;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.WhiteSmoke;
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(AreaButtonTextColour);
            groupBox1.Controls.Add(cbAreaName);
            groupBox1.Controls.Add(AreaBackgroundColour);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(newAreaName);
            groupBox1.Font = new Font("Segoe UI", 8.830189F);
            groupBox1.Location = new Point(21, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(720, 150);
            groupBox1.TabIndex = 27;
            groupBox1.TabStop = false;
            groupBox1.Text = "Area";
            // 
            // groupBox2
            // 
            groupBox2.BackColor = Color.WhiteSmoke;
            groupBox2.Controls.Add(CopyColourFromArea);
            groupBox2.Controls.Add(ButtonToggleTextColour);
            groupBox2.Controls.Add(ButtonToggleBackgroundColour);
            groupBox2.Controls.Add(ButtonTextColour);
            groupBox2.Controls.Add(ButtonBackgroundColour);
            groupBox2.Controls.Add(buttonName);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(buttonCubaseCommands);
            groupBox2.Controls.Add(action);
            groupBox2.Controls.Add(cbButtonType);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label9);
            groupBox2.FlatStyle = FlatStyle.Flat;
            groupBox2.Location = new Point(21, 182);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(720, 315);
            groupBox2.TabIndex = 28;
            groupBox2.TabStop = false;
            groupBox2.Text = "Button";
            // 
            // ButtonToggleTextColour
            // 
            ButtonToggleTextColour.Label = "Button Toggle Text Colour";
            ButtonToggleTextColour.Location = new Point(276, 248);
            ButtonToggleTextColour.Name = "ButtonToggleTextColour";
            ButtonToggleTextColour.Size = new Size(259, 61);
            ButtonToggleTextColour.TabIndex = 28;
            // 
            // ButtonToggleBackgroundColour
            // 
            ButtonToggleBackgroundColour.Label = "Button Toggle Background Colour";
            ButtonToggleBackgroundColour.Location = new Point(24, 248);
            ButtonToggleBackgroundColour.Name = "ButtonToggleBackgroundColour";
            ButtonToggleBackgroundColour.Size = new Size(246, 61);
            ButtonToggleBackgroundColour.TabIndex = 27;
            // 
            // ButtonTextColour
            // 
            ButtonTextColour.Label = "Button Text Colour";
            ButtonTextColour.Location = new Point(276, 171);
            ButtonTextColour.Name = "ButtonTextColour";
            ButtonTextColour.Size = new Size(259, 61);
            ButtonTextColour.TabIndex = 26;
            // 
            // ButtonBackgroundColour
            // 
            ButtonBackgroundColour.Label = "Button Background Colour";
            ButtonBackgroundColour.Location = new Point(24, 171);
            ButtonBackgroundColour.Name = "ButtonBackgroundColour";
            ButtonBackgroundColour.Size = new Size(259, 61);
            ButtonBackgroundColour.TabIndex = 25;
            // 
            // CopyColourFromArea
            // 
            CopyColourFromArea.AutoSize = true;
            CopyColourFromArea.Location = new Point(24, 151);
            CopyColourFromArea.Name = "CopyColourFromArea";
            CopyColourFromArea.Size = new Size(142, 17);
            CopyColourFromArea.TabIndex = 29;
            CopyColourFromArea.TabStop = true;
            CopyColourFromArea.Text = "Copy colour from Area";
            // 
            // AddKeyToCommandsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(779, 570);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(buttonAdd);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "AddKeyToCommandsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Key For Cubase MIDI";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private TextBox buttonName;
        private Label label2;
        private TextBox action;
        private Button buttonAdd;
        private Label label5;
        private ComboBox cbAreaName;
        private Label label6;
        private TextBox newAreaName;
        private Label label9;
        private ComboBox cbButtonType;
        private Button buttonCubaseCommands;
        private Custom.ColourPickerControl AreaBackgroundColour;
        private Custom.ColourPickerControl AreaButtonTextColour;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Custom.ColourPickerControl ButtonBackgroundColour;
        private Custom.ColourPickerControl ButtonTextColour;
        private Custom.ColourPickerControl ButtonToggleBackgroundColour;
        private Custom.ColourPickerControl ButtonToggleTextColour;
        private LinkLabel CopyColourFromArea;
    }
}