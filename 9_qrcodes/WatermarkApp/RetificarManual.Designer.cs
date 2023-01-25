namespace WatermarkApp
{
    partial class RetificarManual
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.finalizar_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14F);
            this.label1.Location = new System.Drawing.Point(445, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(554, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "Por favor clique no centro das posições do Qrcode";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(450, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(909, 933);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_Click);
            // 
            // finalizar_btn
            // 
            this.finalizar_btn.BackColor = System.Drawing.Color.Green;
            this.finalizar_btn.Font = new System.Drawing.Font("Arial", 12F);
            this.finalizar_btn.ForeColor = System.Drawing.SystemColors.Control;
            this.finalizar_btn.Location = new System.Drawing.Point(780, 977);
            this.finalizar_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.finalizar_btn.Name = "finalizar_btn";
            this.finalizar_btn.Size = new System.Drawing.Size(175, 69);
            this.finalizar_btn.TabIndex = 6;
            this.finalizar_btn.Text = "Finalizar";
            this.finalizar_btn.UseVisualStyleBackColor = false;
            this.finalizar_btn.Click += new System.EventHandler(this.finalizar_btn_Click);
            // 
            // RetificarManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1055);
            this.Controls.Add(this.finalizar_btn);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Name = "RetificarManual";
            this.Text = "RetificarManual";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.RetificarManual_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button finalizar_btn;
    }
}