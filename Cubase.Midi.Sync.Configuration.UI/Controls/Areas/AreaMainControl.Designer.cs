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
            areaListView = new AreaListView();
            SuspendLayout();
            // 
            // areaListView
            // 
            areaListView.Dock = DockStyle.Fill;
            areaListView.FullRowSelect = true;
            areaListView.Location = new Point(0, 0);
            areaListView.MultiSelect = false;
            areaListView.Name = "areaListView";
            areaListView.Size = new Size(489, 257);
            areaListView.TabIndex = 0;
            areaListView.UseCompatibleStateImageBehavior = false;
            areaListView.View = View.Details;
            // 
            // AreaMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(areaListView);
            Name = "AreaMainControl";
            Size = new Size(489, 257);
            ResumeLayout(false);
        }

        #endregion

        private AreaListView areaListView;
    }
}
