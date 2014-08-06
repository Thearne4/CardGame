namespace TestUI
{
    partial class FrmLanBrowser
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
            this.dgvLanBrowser = new System.Windows.Forms.DataGridView();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnManualConnect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLanBrowser
            // 
            this.dgvLanBrowser.AllowUserToAddRows = false;
            this.dgvLanBrowser.AllowUserToDeleteRows = false;
            this.dgvLanBrowser.AllowUserToOrderColumns = true;
            this.dgvLanBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLanBrowser.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLanBrowser.Location = new System.Drawing.Point(12, 12);
            this.dgvLanBrowser.MultiSelect = false;
            this.dgvLanBrowser.Name = "dgvLanBrowser";
            this.dgvLanBrowser.ReadOnly = true;
            this.dgvLanBrowser.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLanBrowser.Size = new System.Drawing.Size(589, 528);
            this.dgvLanBrowser.TabIndex = 0;
            this.dgvLanBrowser.SelectionChanged += new System.EventHandler(this.dgvLanBrowser_SelectionChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(607, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(110, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "&Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnManualConnect
            // 
            this.btnManualConnect.Location = new System.Drawing.Point(607, 41);
            this.btnManualConnect.Name = "btnManualConnect";
            this.btnManualConnect.Size = new System.Drawing.Size(110, 23);
            this.btnManualConnect.TabIndex = 1;
            this.btnManualConnect.Text = "&Manual Connect";
            this.btnManualConnect.UseVisualStyleBackColor = true;
            // 
            // FrmLanBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 552);
            this.Controls.Add(this.btnManualConnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.dgvLanBrowser);
            this.Name = "FrmLanBrowser";
            this.Text = "FrmLanBrowser";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanBrowser)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLanBrowser;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnManualConnect;
    }
}