using System.Windows.Forms;

namespace WatermarkApp
{
    partial class Processamento
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Processamento));
            this.doc_label = new System.Windows.Forms.Label();
            this.gerar_btn = new System.Windows.Forms.Button();
            this.rejeitar_btn = new System.Windows.Forms.Button();
            this.aceitar_btn = new System.Windows.Forms.Button();
            this.axAcroPDF1 = new AxAcroPDFLib.AxAcroPDF();
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).BeginInit();
            this.SuspendLayout();
            // 
            // doc_label
            // 
            this.doc_label.AutoSize = true;
            this.doc_label.Font = new System.Drawing.Font("Arial", 12F);
            this.doc_label.Location = new System.Drawing.Point(640, 9);
            this.doc_label.Name = "doc_label";
            this.doc_label.Size = new System.Drawing.Size(115, 23);
            this.doc_label.TabIndex = 9;
            this.doc_label.Text = "Documento:";
            // 
            // gerar_btn
            // 
            this.gerar_btn.BackColor = System.Drawing.Color.Blue;
            this.gerar_btn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gerar_btn.ForeColor = System.Drawing.SystemColors.Control;
            this.gerar_btn.Location = new System.Drawing.Point(335, 849);
            this.gerar_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gerar_btn.Name = "gerar_btn";
            this.gerar_btn.Size = new System.Drawing.Size(175, 69);
            this.gerar_btn.TabIndex = 9;
            this.gerar_btn.Text = "Processar";
            this.gerar_btn.UseVisualStyleBackColor = false;
            this.gerar_btn.Click += new System.EventHandler(this.Gerar_btn_Click);
            // 
            // rejeitar_btn
            // 
            this.rejeitar_btn.BackColor = System.Drawing.Color.Red;
            this.rejeitar_btn.Font = new System.Drawing.Font("Arial", 12F);
            this.rejeitar_btn.ForeColor = System.Drawing.SystemColors.Control;
            this.rejeitar_btn.Location = new System.Drawing.Point(1040, 849);
            this.rejeitar_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rejeitar_btn.Name = "rejeitar_btn";
            this.rejeitar_btn.Size = new System.Drawing.Size(180, 69);
            this.rejeitar_btn.TabIndex = 3;
            this.rejeitar_btn.Text = "Rejeitar";
            this.rejeitar_btn.UseVisualStyleBackColor = false;
            this.rejeitar_btn.Click += new System.EventHandler(this.Rejeitar_btn_Click);
            // 
            // aceitar_btn
            // 
            this.aceitar_btn.BackColor = System.Drawing.Color.Green;
            this.aceitar_btn.Font = new System.Drawing.Font("Arial", 12F);
            this.aceitar_btn.ForeColor = System.Drawing.SystemColors.Control;
            this.aceitar_btn.Location = new System.Drawing.Point(695, 849);
            this.aceitar_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.aceitar_btn.Name = "aceitar_btn";
            this.aceitar_btn.Size = new System.Drawing.Size(175, 69);
            this.aceitar_btn.TabIndex = 5;
            this.aceitar_btn.Text = "Aceitar";
            this.aceitar_btn.UseVisualStyleBackColor = false;
            this.aceitar_btn.Click += new System.EventHandler(this.Aceitar_btn_Click);
            // 
            // axAcroPDF1
            // 
            this.axAcroPDF1.Enabled = true;
            this.axAcroPDF1.Location = new System.Drawing.Point(175, 35);
            this.axAcroPDF1.Name = "axAcroPDF1";
            this.axAcroPDF1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAcroPDF1.OcxState")));
            this.axAcroPDF1.Size = new System.Drawing.Size(1084, 629);
            this.axAcroPDF1.TabIndex = 10;
            // 
            // Processamento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1055);
            this.Controls.Add(this.axAcroPDF1);
            this.Controls.Add(this.gerar_btn);
            this.Controls.Add(this.doc_label);
            this.Controls.Add(this.rejeitar_btn);
            this.Controls.Add(this.aceitar_btn);
            this.Name = "Processamento";
            this.Text = "Watermark";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label doc_label;
        private System.Windows.Forms.Button gerar_btn;
        private Button rejeitar_btn;
        private Button aceitar_btn;
        private AxAcroPDFLib.AxAcroPDF axAcroPDF1;
    }
}

