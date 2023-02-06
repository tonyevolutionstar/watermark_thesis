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
        private int sizeCircleX;
        private int countQrcodes = 0; // até 9
        List<Point> positionsQrcodes;
        string positionsPrint = "";

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
            for(int i = 0; i < positionsQrcodes.Count; i++)
            {
                positionsPrint += positionsQrcodes[i].X + "," + positionsQrcodes[i].Y + "|";
            }
            

            if (countQrcodes == 9)
            {
                Commom commom = new Commom();
                string positionsClicked = positionsPrint.Substring(0, positionsPrint.Length - 1);
                MessageBox.Show("Podes finalizar, com as posições " + positionsClicked);
                Bitmap bmp = new Bitmap(img_file);

                SQL_connection sql = new SQL_connection();
                string orignal_positions = sql.Get_Positions_CircleX(id_doc);

                string[] ori_pos = orignal_positions.Split('|');
                string[] cli_pos = positionsClicked.Split('|');

                string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Bitmap x_circle = new Bitmap(partialPath + @"\number_qrcode\X_resized.png");

                for (int i = 0; i < 9; i++)
                {
                    int j = i + 1;
                    if (j == 9)
                        j = 7;

                    string[] ori_p = ori_pos[i].Split(',');
                    string[] ori2_p = ori_pos[j].Split(',');
                    string[] cli_p = cli_pos[i].Split(',');
                    string[] cli2_p = cli_pos[j].Split(',');

                    Point p1 = new Point(int.Parse(ori_p[0]), int.Parse(ori_p[1]));
                    Point p2 = new Point(int.Parse(ori2_p[0]), int.Parse(ori2_p[1]));

                    int lx = p1.X - p2.X;
                    int ly = p1.Y - p2.Y;

                    int dx = p1.X - x_circle.Width;
                    int dy = p1.Y - x_circle.Height;

                    Point p1_l = new Point(int.Parse(cli_p[0]), int.Parse(cli_p[1]));
                    Point p2_l = new Point(int.Parse(cli2_p[0]), int.Parse(cli2_p[1]));

                    int lx_l = p1_l.X - p2_l.X;
                    int ly_l = p1_l.Y - p2_l.Y;

                    if (lx == 0)
                    {
                        lx = 1;
                    }else if (ly == 0)
                    {
                        ly = 1;
                    }
                    else if (ly_l == 0)
                    {
                        ly_l = 1;
                    }
                    else if (lx_l == 0)
                    {
                        lx_l = 1;
                    }
                               
                    int dx_l = dx * lx_l / lx;
                    int dy_l = dy * ly_l / ly;

                    Point p_new = new Point(p1_l.X + dx_l, p2_l.Y + dy_l);
                    Console.WriteLine("i,j:" + i.ToString() + "," + j.ToString() + " Translate point " + p_new.ToString());
       
                }

                Console.WriteLine("Posições originais " + orignal_positions + " \n posições clicadas " + positionsClicked);

                AuxFunc auxfunc = new AuxFunc(file_name, sql, id_doc, sizeCircleX);
                auxfunc.CalculateIntersection(positionsClicked, file_name);
                Console.WriteLine("Intersection Done");

                commom.retificarAnalise(id_doc, sql, file_name, sizeCircleX);
                MessageBox.Show("Posições originais " + orignal_positions + " \n posições clicadas " + positionsClicked);
                bmp.Dispose();
            }
            else
            {
                MessageBox.Show("Erro, clicaste mais vezes na imagem, ou não clicaste em todos os qrcodes, clica no centro dos 9 qrcodes");
                countQrcodes = 0;
                positionsQrcodes = new List<Point>();
                positionsPrint = "";
            }
        }
    }
}
