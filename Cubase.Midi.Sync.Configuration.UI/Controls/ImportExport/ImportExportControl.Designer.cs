namespace Cubase.Midi.Sync.Configuration.UI.Controls.ImportExport
{
    partial class ImportExportControl
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
            DateFilter = new Panel();
            filterDateLabel = new Label();
            FilterDate = new DateTimePicker();
            ImportButton = new Button();
            ExportButton = new Button();
            ListViewPanel = new Panel();
            importExportListView = new ImportExportListView();
            DateFilter.SuspendLayout();
            ListViewPanel.SuspendLayout();
            SuspendLayout();
            // 
            // DateFilter
            // 
            DateFilter.Controls.Add(filterDateLabel);
            DateFilter.Controls.Add(FilterDate);
            DateFilter.Controls.Add(ImportButton);
            DateFilter.Controls.Add(ExportButton);
            DateFilter.Dock = DockStyle.Bottom;
            DateFilter.Location = new Point(0, 374);
            DateFilter.Name = "DateFilter";
            DateFilter.Size = new Size(693, 66);
            DateFilter.TabIndex = 1;
            // 
            // filterDateLabel
            // 
            filterDateLabel.AutoSize = true;
            filterDateLabel.Location = new Point(129, 23);
            filterDateLabel.Name = "filterDateLabel";
            filterDateLabel.Size = new Size(71, 17);
            filterDateLabel.TabIndex = 3;
            filterDateLabel.Text = "Filter After:";
            // 
            // FilterDate
            // 
            FilterDate.Location = new Point(206, 19);
            FilterDate.Name = "FilterDate";
            FilterDate.Size = new Size(221, 25);
            FilterDate.TabIndex = 2;
            // 
            // ImportButton
            // 
            ImportButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ImportButton.Location = new Point(591, 19);
            ImportButton.Name = "ImportButton";
            ImportButton.Size = new Size(83, 25);
            ImportButton.TabIndex = 1;
            ImportButton.Text = "Import";
            ImportButton.UseVisualStyleBackColor = true;
            // 
            // ExportButton
            // 
            ExportButton.Location = new Point(21, 19);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new Size(83, 25);
            ExportButton.TabIndex = 0;
            ExportButton.Text = "Export";
            ExportButton.UseVisualStyleBackColor = true;
            // 
            // ListViewPanel
            // 
            ListViewPanel.Controls.Add(importExportListView);
            ListViewPanel.Dock = DockStyle.Fill;
            ListViewPanel.Location = new Point(0, 0);
            ListViewPanel.Name = "ListViewPanel";
            ListViewPanel.Size = new Size(693, 374);
            ListViewPanel.TabIndex = 2;
            // 
            // importExportListView
            // 
            importExportListView.CheckBoxes = true;
            importExportListView.Dock = DockStyle.Fill;
            importExportListView.Location = new Point(0, 0);
            importExportListView.Name = "importExportListView";
            importExportListView.Size = new Size(693, 374);
            importExportListView.TabIndex = 0;
            importExportListView.UseCompatibleStateImageBehavior = false;
            importExportListView.View = View.Details;
            // 
            // ImportExportControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ListViewPanel);
            Controls.Add(DateFilter);
            Name = "ImportExportControl";
            Size = new Size(693, 440);
            DateFilter.ResumeLayout(false);
            DateFilter.PerformLayout();
            ListViewPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Panel DateFilter;
        private Panel ListViewPanel;
        private ImportExportListView importExportListView;
        private Button ExportButton;
        private Button ImportButton;
        private DateTimePicker FilterDate;
        private Label filterDateLabel;
    }
}
