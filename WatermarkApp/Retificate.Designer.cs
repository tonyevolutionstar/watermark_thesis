namespace WatermarkApp
{
    partial class Retificate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Retificate));
            this.label1 = new System.Windows.Forms.Label();
            this.dct_name = new System.Windows.Forms.Label();
            this.file_qrcode = new AxAcroPDFLib.AxAcroPDF();
            this.label2 = new System.Windows.Forms.Label();
            this.user = new System.Windows.Forms.Label();
            this.sigla = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.posto = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.forense_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.file_qrcode)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(565, 719);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Documento:";
            // 
            // dct_name
            // 
            this.dct_name.AutoSize = true;
            this.dct_name.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dct_name.Location = new System.Drawing.Point(728, 720);
            this.dct_name.Name = "dct_name";
            this.dct_name.Size = new System.Drawing.Size(71, 26);
            this.dct_name.TabIndex = 1;
            this.dct_name.Text = "label2";
            // 
            // file_qrcode
            // 
            this.file_qrcode.Enabled = true;
            this.file_qrcode.Location = new System.Drawing.Point(272, 12);
            this.file_qrcode.Name = "file_qrcode";
            this.file_qrcode.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("file_qrcode.OcxState")));
            this.file_qrcode.Size = new System.Drawing.Size(1000, 533);
            this.file_qrcode.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(565, 770);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 27);
            this.label2.TabIndex = 5;
            this.label2.Text = "Utilizador:";
            // 
            // user
            // 
            this.user.AutoSize = true;
            this.user.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.user.Location = new System.Drawing.Point(728, 770);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(71, 26);
            this.user.TabIndex = 6;
            this.user.Text = "label2";
            // 
            // sigla
            // 
            this.sigla.AutoSize = true;
            this.sigla.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sigla.Location = new System.Drawing.Point(728, 827);
            this.sigla.Name = "sigla";
            this.sigla.Size = new System.Drawing.Size(71, 26);
            this.sigla.TabIndex = 8;
            this.sigla.Text = "label2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(565, 827);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 27);
            this.label4.TabIndex = 7;
            this.label4.Text = "Sigla:";
            // 
            // posto
            // 
            this.posto.AutoSize = true;
            this.posto.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posto.Location = new System.Drawing.Point(729, 890);
            this.posto.Name = "posto";
            this.posto.Size = new System.Drawing.Size(71, 26);
            this.posto.TabIndex = 10;
            this.posto.Text = "label2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(566, 888);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 27);
            this.label5.TabIndex = 9;
            this.label5.Text = "Posto Atual:";
            // 
            // forense_btn
            // 
            this.forense_btn.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.forense_btn.Location = new System.Drawing.Point(1644, 832);
            this.forense_btn.Name = "forense_btn";
            this.forense_btn.Size = new System.Drawing.Size(246, 84);
            this.forense_btn.TabIndex = 11;
            this.forense_btn.Text = "Análise Forense";
            this.forense_btn.UseVisualStyleBackColor = true;
            this.forense_btn.Click += new System.EventHandler(this.Forense_btn_Click);
            // 
            // Retificate
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 993);
            this.Controls.Add(this.forense_btn);
            this.Controls.Add(this.posto);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sigla);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.user);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.file_qrcode);
            this.Controls.Add(this.dct_name);
            this.Controls.Add(this.label1);
            this.Name = "Retificate";
            this.Text = "Retificar";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Retificate_FormClosed);
            this.Load += new System.EventHandler(this.Retificate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.file_qrcode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label dct_name;
        private AxAcroPDFLib.AxAcroPDF file_qrcode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label user;
        private System.Windows.Forms.Label sigla;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label posto;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button forense_btn;
    }
}