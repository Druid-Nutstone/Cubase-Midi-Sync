namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas
{
    partial class AreaMainControl
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
            AddCategory = new Button();
            MoveDown = new Button();
            MoveUp = new Button();
            NewCategoryText = new TextBox();
            label2 = new Label();
            ButtonCategoryListBox = new ListBox();
            label1 = new Label();
            areaListView = new AreaListView();
            DeleteCategory = new Button();
            checkBox1 = new CheckBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Window;
            panel1.Controls.Add(checkBox1);
            panel1.Controls.Add(DeleteCategory);
            panel1.Controls.Add(AddCategory);
            panel1.Controls.Add(MoveDown);
            panel1.Controls.Add(MoveUp);
            panel1.Controls.Add(NewCategoryText);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(ButtonCategoryListBox);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(489, 149);
            panel1.TabIndex = 0;
            // 
            // AddCategory
            // 
            AddCategory.Location = new Point(281, 74);
            AddCategory.Name = "AddCategory";
            AddCategory.Size = new Size(83, 25);
            AddCategory.TabIndex = 7;
            AddCategory.Text = "Add";
            AddCategory.UseVisualStyleBackColor = true;
            // 
            // MoveDown
            // 
            MoveDown.Location = new Point(172, 69);
            MoveDown.Name = "MoveDown";
            MoveDown.Size = new Size(58, 25);
            MoveDown.TabIndex = 6;
            MoveDown.Text = "Down";
            MoveDown.UseVisualStyleBackColor = true;
            // 
            // MoveUp
            // 
            MoveUp.Location = new Point(172, 38);
            MoveUp.Name = "MoveUp";
            MoveUp.Size = new Size(58, 25);
            MoveUp.TabIndex = 5;
            MoveUp.Text = "Up";
            MoveUp.UseVisualStyleBackColor = true;
            // 
            // NewCategoryText
            // 
            NewCategoryText.Location = new Point(281, 38);
            NewCategoryText.Name = "NewCategoryText";
            NewCategoryText.Size = new Size(158, 25);
            NewCategoryText.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label2.Location = new Point(281, 18);
            label2.Name = "label2";
            label2.Size = new Size(95, 17);
            label2.TabIndex = 2;
            label2.Text = "New Category";
            // 
            // ButtonCategoryListBox
            // 
            ButtonCategoryListBox.FormattingEnabled = true;
            ButtonCategoryListBox.Location = new Point(34, 38);
            ButtonCategoryListBox.Name = "ButtonCategoryListBox";
            ButtonCategoryListBox.Size = new Size(132, 89);
            ButtonCategoryListBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold);
            label1.Location = new Point(30, 16);
            label1.Name = "label1";
            label1.Size = new Size(127, 17);
            label1.TabIndex = 0;
            label1.Text = "Button Catgegories";
            // 
            // areaListView
            // 
            areaListView.Dock = DockStyle.Fill;
            areaListView.FullRowSelect = true;
            areaListView.Location = new Point(0, 149);
            areaListView.MultiSelect = false;
            areaListView.Name = "areaListView";
            areaListView.Size = new Size(489, 194);
            areaListView.TabIndex = 1;
            areaListView.UseCompatibleStateImageBehavior = false;
            areaListView.View = View.Details;
            // 
            // DeleteCategory
            // 
            DeleteCategory.Location = new Point(172, 100);
            DeleteCategory.Name = "DeleteCategory";
            DeleteCategory.Size = new Size(58, 25);
            DeleteCategory.TabIndex = 8;
            DeleteCategory.Text = "Delete";
            DeleteCategory.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(416, 131);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(87, 21);
            checkBox1.TabIndex = 9;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // AreaMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(areaListView);
            Controls.Add(panel1);
            Name = "AreaMainControl";
            Size = new Size(489, 343);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private AreaListView areaListView;
        private Label label1;
        private ListBox ButtonCategoryListBox;
        private TextBox NewCategoryText;
        private Label label2;
        private Button MoveUp;
        private Button MoveDown;
        private Button AddCategory;
        private CheckBox checkBox1;
        private Button DeleteCategory;
    }
}
