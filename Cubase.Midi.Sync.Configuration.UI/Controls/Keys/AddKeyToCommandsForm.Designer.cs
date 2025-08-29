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
            toggleButton = new CheckBox();
            label3 = new Label();
            label4 = new Label();
            backgroundColour = new TextBox();
            textColour = new TextBox();
            backgroundColourButton = new Button();
            textColourButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(55, 30);
            label1.Name = "label1";
            label1.Size = new Size(183, 17);
            label1.TabIndex = 0;
            label1.Text = "Description (Button Header)";
            // 
            // buttonName
            // 
            buttonName.Location = new Point(58, 52);
            buttonName.Name = "buttonName";
            buttonName.Size = new Size(224, 25);
            buttonName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(315, 30);
            label2.Name = "label2";
            label2.Size = new Size(74, 17);
            label2.TabIndex = 2;
            label2.Text = "Key Action";
            // 
            // action
            // 
            action.Location = new Point(315, 52);
            action.Name = "action";
            action.Size = new Size(221, 25);
            action.TabIndex = 3;
            // 
            // buttonAdd
            // 
            buttonAdd.Location = new Point(61, 198);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(83, 25);
            buttonAdd.TabIndex = 4;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            // 
            // toggleButton
            // 
            toggleButton.AutoSize = true;
            toggleButton.Location = new Point(576, 114);
            toggleButton.Name = "toggleButton";
            toggleButton.Size = new Size(104, 21);
            toggleButton.TabIndex = 5;
            toggleButton.Text = "ToggleButton";
            toggleButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label3.Location = new Point(55, 114);
            label3.Name = "label3";
            label3.Size = new Size(127, 17);
            label3.TabIndex = 6;
            label3.Text = "BackGround Colour";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label4.Location = new Point(315, 114);
            label4.Name = "label4";
            label4.Size = new Size(79, 17);
            label4.TabIndex = 7;
            label4.Text = "Text Colour";
            // 
            // backgroundColour
            // 
            backgroundColour.Location = new Point(59, 145);
            backgroundColour.Name = "backgroundColour";
            backgroundColour.Size = new Size(153, 25);
            backgroundColour.TabIndex = 8;
            // 
            // textColour
            // 
            textColour.Location = new Point(319, 144);
            textColour.Name = "textColour";
            textColour.Size = new Size(159, 25);
            textColour.TabIndex = 9;
            // 
            // backgroundColourButton
            // 
            backgroundColourButton.Location = new Point(218, 144);
            backgroundColourButton.Name = "backgroundColourButton";
            backgroundColourButton.Size = new Size(53, 25);
            backgroundColourButton.TabIndex = 10;
            backgroundColourButton.Text = "Pick";
            backgroundColourButton.UseVisualStyleBackColor = true;
            // 
            // textColourButton
            // 
            textColourButton.Location = new Point(484, 145);
            textColourButton.Name = "textColourButton";
            textColourButton.Size = new Size(52, 25);
            textColourButton.TabIndex = 11;
            textColourButton.Text = "Pick";
            textColourButton.UseVisualStyleBackColor = true;
            // 
            // AddKeyToCommandsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(715, 261);
            Controls.Add(textColourButton);
            Controls.Add(backgroundColourButton);
            Controls.Add(textColour);
            Controls.Add(backgroundColour);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(toggleButton);
            Controls.Add(buttonAdd);
            Controls.Add(action);
            Controls.Add(label2);
            Controls.Add(buttonName);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "AddKeyToCommandsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Key For Cubase MIDI";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox buttonName;
        private Label label2;
        private TextBox action;
        private Button buttonAdd;
        private CheckBox toggleButton;
        private Label label3;
        private Label label4;
        private TextBox backgroundColour;
        private TextBox textColour;
        private Button backgroundColourButton;
        private Button textColourButton;
    }
}