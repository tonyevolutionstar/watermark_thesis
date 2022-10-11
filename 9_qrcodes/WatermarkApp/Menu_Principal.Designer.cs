namespace WatermarkApp
{
    partial class Menu_Principal
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
            this.processar_btn = new System.Windows.Forms.Button();
            this.retificar_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // processar_btn
            // 
            this.processar_btn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processar_btn.Location = new System.Drawing.Point(147, 153);
            this.processar_btn.Name = "processar_btn";
            this.processar_btn.Size = new System.Drawing.Size(141, 61);
            this.processar_btn.TabIndex = 1;
            this.processar_btn.Text = "Processar";
            this.processar_btn.UseVisualStyleBackColor = true;
            this.processar_btn.Click += new System.EventHandler(this.Processar_btn_Click);
            // 
            // retificar_btn
            // 
            this.retificar_btn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.retificar_btn.Location = new System.Drawing.Point(147, 380);
            this.retificar_btn.Name = "retificar_btn";
            this.retificar_btn.Size = new System.Drawing.Size(141, 61);
            this.retificar_btn.TabIndex = 2;
            this.retificar_btn.Text = "Retificar";
            this.retificar_btn.UseVisualStyleBackColor = true;
            this.retificar_btn.Click += new System.EventHandler(this.Retificar_btn_Click);
            // 
            // Menu_Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 604);
            this.Controls.Add(this.retificar_btn);
            this.Controls.Add(this.processar_btn);
            this.Name = "Menu_Principal";
            this.Text = "Menu Principal";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button processar_btn;
        private System.Windows.Forms.Button retificar_btn;
    }
}