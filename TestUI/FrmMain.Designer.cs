namespace TestUI
{
    partial class FrmMain
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
            this.btnLocal = new System.Windows.Forms.Button();
            this.btnLan = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLocal
            // 
            this.btnLocal.Enabled = false;
            this.btnLocal.Location = new System.Drawing.Point(12, 12);
            this.btnLocal.Name = "btnLocal";
            this.btnLocal.Size = new System.Drawing.Size(92, 23);
            this.btnLocal.TabIndex = 0;
            this.btnLocal.Text = "Local";
            this.btnLocal.UseVisualStyleBackColor = true;
            // 
            // btnLan
            // 
            this.btnLan.Location = new System.Drawing.Point(12, 41);
            this.btnLan.Name = "btnLan";
            this.btnLan.Size = new System.Drawing.Size(92, 23);
            this.btnLan.TabIndex = 1;
            this.btnLan.Text = "LAN";
            this.btnLan.UseVisualStyleBackColor = true;
            this.btnLan.Click += new System.EventHandler(this.btnLan_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(12, 70);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(92, 23);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(116, 105);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnLan);
            this.Controls.Add(this.btnLocal);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLocal;
        private System.Windows.Forms.Button btnLan;
        private System.Windows.Forms.Button btnSettings;
    }
}

