using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class RetificarManual : Form
    {
        private string file_name;
        private string img_file;
        private string result_barcode;
        private int id_doc; 
        private int sizeCircleX;
        private int countCirclesX = 0;
        List<Point> positionsCirclesX;
        string positionsPrint = "";
        private string errorNotEnoughtData = "Erro, clicaste mais vezes na imagem, ou não clicaste em todos os qrcodes, clica no centro dos 9 circulos com x";

        public RetificarManual(string file_name, int size_qrcode)
        {
            InitializeComponent();
            this.file_name = file_name;
            Commom commom = new Commom();
            img_file = commom.Convert_pdf_png(file_name);
            result_barcode = commom.Read_barcode(file_name);
            string[] resultado = result_barcode.Split(';');
            id_doc = Int32.Parse(resultado[0]);
            this.sizeCircleX = size_qrcode;
            positionsCirclesX = new List<Point>();  
        }

        private void RetificarManual_Load(object o, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(img_file);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; 
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
            countCirclesX++;
            Point p = new Point(e.X, e.Y);
            MessageBox.Show("Cliquei no ponto " + p.ToString() + countCirclesX.ToString() + " cliques");
            if (countCirclesX >= 0 && countCirclesX <= 9)
                positionsCirclesX.Add(p);
        }


        private void finalizar_btn_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < positionsCirclesX.Count; i++)
            {
                positionsPrint += positionsCirclesX[i].X + "," + positionsCirclesX[i].Y + "|";
            }
            
            if (countCirclesX == 9)
            {
                Commom commom = new Commom();
                string positionsClicked = positionsPrint.Substring(0, positionsPrint.Length - 1);
                MessageBox.Show("Podes finalizar, com as posições " + positionsClicked);
                Bitmap bmp = new Bitmap(img_file);

                SQL_connection sql = new SQL_connection();
                string orignal_positions = sql.Get_Positions_CircleX(id_doc);

                string[] ori_pos = orignal_positions.Split('|');
                string[] cli_pos = positionsClicked.Split('|');

                string new_points = "";

                for (int i = 0; i < 9; i++)
                {
                    string[] ori_p = ori_pos[i].Split(',');
                    string[] cli_p = cli_pos[i].Split(',');

                    Point p1 = new Point(int.Parse(ori_p[0]), int.Parse(ori_p[1]));
                    Point p2 = new Point(int.Parse(cli_p[0]), int.Parse(cli_p[1]));
                    
                    if(p2.X > p1.X && p2.Y < p1.Y)
                    {
                        Point p_new = new Point(p2.X - 12, p2.Y + 12);
                        new_points += p_new.X + "," + p_new.Y + "|";
                    }
                    else if (p2.X > p1.X && p2.Y > p1.Y)
                    {
                        Point p_new = new Point(p2.X - 12, p2.Y - 12);
                        new_points += p_new.X + "," + p_new.Y + "|";
                    }
                    else if (p2.X < p1.X && p2.Y > p1.Y)
                    {
                        Point p_new = new Point(p2.X + 12, p2.Y - 12);
                        new_points += p_new.X + "," + p_new.Y + "|";
                    }
                    else if (p2.X < p1.X && p2.Y < p1.Y)
                    {
                        Point p_new = new Point(p2.X + 12, p2.Y + 12);
                        new_points += p_new.X + "," + p_new.Y + "|";
                    }
                }

                string positionsFixed = new_points.Substring(0, positionsPrint.Length - 1);

                AuxFunc auxfunc = new AuxFunc(file_name, sql, id_doc, sizeCircleX);
                auxfunc.CalculateIntersection(positionsFixed, file_name);

                commom.retificarAnalise(id_doc, sql, file_name, sizeCircleX);
                MessageBox.Show("Posições originais " + orignal_positions + " \n Posições clicadas " + positionsClicked + " \n Posições corrigidas " + positionsFixed);
                bmp.Dispose();
            }
            else
            {
                MessageBox.Show(errorNotEnoughtData);
                countCirclesX = 0;
                positionsCirclesX = new List<Point>();
                positionsPrint = "";
            }
        }
    }
}
