using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class RetificarManual : Form
    {
        private string img_file;
        private int size_qrcode;
        private int countQrcodes = 0; // até 9
        List<Point> positionsQrcodes;

        public RetificarManual(string file_name, int size_qrcode)
        {
            InitializeComponent();
            Commom commom = new Commom();
            img_file = commom.Convert_pdf_png(file_name);
            size_qrcode = size_qrcode;
            positionsQrcodes = new List<Point>();
        }

 
        private void RetificarManual_Load(object o, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(img_file);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; 
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
            countQrcodes++;
            Point p = new Point(e.X, e.Y);
            MessageBox.Show("Cliquei no ponto " + p.ToString() + " o total de cliques está em " + countQrcodes.ToString());
            if (countQrcodes >= 0 && countQrcodes <= 9)
                positionsQrcodes.Add(p);
        }

        private void finalizar_btn_Click(object sender, EventArgs e)
        {
            string positionsPrint = "";

            for(int i = 0; i < positionsQrcodes.Count; i++)
            {
                positionsPrint += positionsQrcodes[i].ToString() + ", ";
            }



            if (countQrcodes == 9)
            {

                MessageBox.Show("Podes finalizar, com as posições " + positionsPrint.Substring(0, positionsPrint.Length - 2));
            }
            else
            {
                MessageBox.Show("Erro, clicaste mais vezes na imagem, ou não clicaste em todos os qrcodes, clica no centro dos 9 qrcodes");
                countQrcodes = 0;
            }

        }
    }
}
