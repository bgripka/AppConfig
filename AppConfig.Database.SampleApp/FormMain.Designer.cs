namespace AppConfig.Database.SampleApp
{
    partial class FormMain
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
            this.cmbCallFunction = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbTextResults = new System.Windows.Forms.RichTextBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabObjectView = new System.Windows.Forms.TabPage();
            this.tvObjectView = new System.Windows.Forms.TreeView();
            this.tabGridView = new System.Windows.Forms.TabPage();
            this.dgvGridView = new System.Windows.Forms.DataGridView();
            this.tabTextView = new System.Windows.Forms.TabPage();
            this.tcMain.SuspendLayout();
            this.tabObjectView.SuspendLayout();
            this.tabGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGridView)).BeginInit();
            this.tabTextView.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbCallFunction
            // 
            this.cmbCallFunction.FormattingEnabled = true;
            this.cmbCallFunction.Items.AddRange(new object[] {
            "Get Product by Name",
            "Order By Product Name",
            "Save New Product"});
            this.cmbCallFunction.Location = new System.Drawing.Point(129, 12);
            this.cmbCallFunction.Name = "cmbCallFunction";
            this.cmbCallFunction.Size = new System.Drawing.Size(184, 21);
            this.cmbCallFunction.TabIndex = 0;
            this.cmbCallFunction.SelectedIndexChanged += new System.EventHandler(this.cmbCallFunction_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Call Example Function";
            // 
            // rtbTextResults
            // 
            this.rtbTextResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbTextResults.Location = new System.Drawing.Point(0, 3);
            this.rtbTextResults.Name = "rtbTextResults";
            this.rtbTextResults.Size = new System.Drawing.Size(536, 322);
            this.rtbTextResults.TabIndex = 2;
            this.rtbTextResults.Text = "";
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tabObjectView);
            this.tcMain.Controls.Add(this.tabGridView);
            this.tcMain.Controls.Add(this.tabTextView);
            this.tcMain.Location = new System.Drawing.Point(2, 39);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(547, 354);
            this.tcMain.TabIndex = 3;
            // 
            // tabObjectView
            // 
            this.tabObjectView.Controls.Add(this.tvObjectView);
            this.tabObjectView.Location = new System.Drawing.Point(4, 22);
            this.tabObjectView.Name = "tabObjectView";
            this.tabObjectView.Padding = new System.Windows.Forms.Padding(3);
            this.tabObjectView.Size = new System.Drawing.Size(539, 328);
            this.tabObjectView.TabIndex = 0;
            this.tabObjectView.Text = "Object View";
            this.tabObjectView.UseVisualStyleBackColor = true;
            // 
            // tvObjectView
            // 
            this.tvObjectView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvObjectView.Location = new System.Drawing.Point(3, 6);
            this.tvObjectView.Name = "tvObjectView";
            this.tvObjectView.Size = new System.Drawing.Size(530, 318);
            this.tvObjectView.TabIndex = 0;
            // 
            // tabGridView
            // 
            this.tabGridView.Controls.Add(this.dgvGridView);
            this.tabGridView.Location = new System.Drawing.Point(4, 22);
            this.tabGridView.Name = "tabGridView";
            this.tabGridView.Padding = new System.Windows.Forms.Padding(3);
            this.tabGridView.Size = new System.Drawing.Size(539, 328);
            this.tabGridView.TabIndex = 1;
            this.tabGridView.Text = "Grid View";
            this.tabGridView.UseVisualStyleBackColor = true;
            // 
            // dgvGridView
            // 
            this.dgvGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGridView.Location = new System.Drawing.Point(3, 6);
            this.dgvGridView.Name = "dgvGridView";
            this.dgvGridView.Size = new System.Drawing.Size(530, 316);
            this.dgvGridView.TabIndex = 0;
            // 
            // tabTextView
            // 
            this.tabTextView.Controls.Add(this.rtbTextResults);
            this.tabTextView.Location = new System.Drawing.Point(4, 22);
            this.tabTextView.Name = "tabTextView";
            this.tabTextView.Size = new System.Drawing.Size(539, 328);
            this.tabTextView.TabIndex = 2;
            this.tabTextView.Text = "Text View";
            this.tabTextView.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 397);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbCallFunction);
            this.Name = "FormMain";
            this.Text = "AppConfig.Database Sample App";
            this.tcMain.ResumeLayout(false);
            this.tabObjectView.ResumeLayout(false);
            this.tabGridView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGridView)).EndInit();
            this.tabTextView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbCallFunction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbTextResults;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabObjectView;
        private System.Windows.Forms.TreeView tvObjectView;
        private System.Windows.Forms.TabPage tabGridView;
        private System.Windows.Forms.DataGridView dgvGridView;
        private System.Windows.Forms.TabPage tabTextView;
    }
}

