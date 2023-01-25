using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class RetificarManual : Form
    {

        private string file_name;
        private string img_file;
        private string result_barcode;
        private int id_doc; 
        private int size_qrcode;
        private int countQrcodes = 0; // até 9
        List<Point> positionsQrcodes;
      

        public RetificarManual(string file_name, int size_qrcode)
        {
            InitializeComponent();
            this.file_name = file_name;
            Commom commom = new Commom();
            img_file = commom.Convert_pdf_png(file_name);
            result_barcode = commom.Read_barcode(file_name);
            string[] resultado = result_barcode.Split(';');
            id_doc = Int32.Parse(resultado[0]);

            this.size_qrcode = size_qrcode;
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

        private async void finalizar_btn_Click(object sender, EventArgs e)
        {
            string positionsPrint = "";

            for(int i = 0; i < positionsQrcodes.Count; i++)
            {
                positionsPrint += positionsQrcodes[i].X + "," + positionsQrcodes[i].Y + "|";
            }


            if (countQrcodes == 9)
            {
                MessageBox.Show("Podes finalizar, com as posições " + positionsPrint.Substring(0, positionsPrint.Length - 1));
                Bitmap bmp = new Bitmap(img_file);
                SQL_connection sql = new SQL_connection();
                AuxFunc auxfunc = new AuxFunc(file_name, sql,id_doc, size_qrcode);
                auxfunc.CalculateIntersection(positionsPrint, file_name);
                Console.WriteLine("Intersection Done");
                Commom commom = new Commom();
                commom.retificarAnalise(id_doc, sql, file_name, size_qrcode);

                bmp.Dispose();
            }
            else
            {
                MessageBox.Show("Erro, clicaste mais vezes na imagem, ou não clicaste em todos os qrcodes, clica no centro dos 9 qrcodes");
                countQrcodes = 0;
            }

        }
    }
}
