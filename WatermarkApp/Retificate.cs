using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class Retificate : Form
    {
        //or - original - o documento que está na base de dados
        //dig - digital - o documento digatalizado que apresenta na aplicação
        private string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private string infoAnaliseForense = "Procedendo à Análise Forense, aguarde!";
        private string error_readBarcode = "Não consegui ler o código de barras";

        private Commom commom;
        private string file_name;
        private readonly string result_barcode;
        private int id_doc;
        #region points
        private Point p1_or;
        private Point p2_or;
        private Point p1_dig;
        private Point p2_dig;

        private Point p1_39_or;
        private Point p2_39_or;
        private Point p1_39_dig;
        private Point p2_39_dig;

        private Point doc_dim_or;
        private Point doc_dim_dig;
        private Point image_dim_or;
        private Point image_dim_dig;

        private Point diff_barcode;
        private Point diff_doc_dim;
        private Point diff_img_dim;
        private PointF prop;

        private string barcode128_or;
        private string barcode39_or;
        private string barcode128_dig;
        private string barcode39_dig;
        #endregion;
        private string dimensions;
        private string scan_dimen;

        private string diff_dimen;
        private string diff_bmp_dim;
        private decimal scale_doc;

        private int delta_y_or;
        private int delta_y_dig;

        private TrackerServices tracker = new TrackerServices();
        private string img_file;

        /// <summary>
        /// Retifica um documento com marca de água
        /// </summary>
        /// <param name="file_name"></param>

        [Obsolete]
        public Retificate(string file_name)
        {
            tracker.WriteFile("---------- Retificar -----------");
            tracker.WriteFile($"retificação do ficheiro {file_name} a iniciar");
            InitializeComponent();
            commom = new Commom();
            this.file_name = file_name;
            img_file = commom.Convert_pdf_png(file_name);

            file_watermark.src = file_name;
            Controls.Add(file_watermark);
            result_barcode = commom.Read_barcode(img_file);
            Console.WriteLine($"Res bar {result_barcode}");
            if (result_barcode == commom.errorReadBarcode)
            {
                tracker.WriteFile($"retificação do ficheiro {file_name} falhou - {error_readBarcode}");
                MessageBox.Show(error_readBarcode);
                Close();
                Dispose();
            }
        }

        /// <summary>
        /// Cria uma imagem com os pontos lidos dos códigos de barras do scan e do original
        /// </summary>
        private void Draw_Points_Barcodes()
        {
            //arc
            int w_arc = 5;
            int h_arc = 5;
            int startAngle = 0;
            int sweepAngle = 360;
            Pen yellow = new Pen(Color.Yellow, 3);
            Pen red = new Pen(Color.Red, 3);
            Pen green = new Pen(Color.Green, 3);
            Pen pink = new Pen(Color.Pink, 3);

            using (Bitmap bmp = new Bitmap(img_file))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    SolidBrush drawBrush = new SolidBrush(Color.Chocolate);
                    Font drawFont = new Font("Arial", 10);

                    int p_x = p1_dig.X * bmp.Width / commom.width;
                    int p_y = p1_dig.Y * bmp.Height / commom.height;
                    int p2_x = p2_dig.X * bmp.Width / commom.width;
                    int p2_y = p2_dig.Y * bmp.Height / commom.height;
                    Point p1_l_u_r = new Point(p_x, p_y);
                    Point p1_r_u_r = new Point(p2_x, p_y);
                    Point p1_r_b_r = new Point(p2_x, p2_y);
                    Point p1_l_b_r = new Point(p_x, p2_y);

                    g.DrawArc(yellow, p1_l_u_r.X, p1_l_u_r.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(yellow, p1_r_u_r.X, p1_r_u_r.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(yellow, p1_l_b_r.X, p1_l_b_r.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(yellow, p1_r_b_r.X, p1_r_b_r.Y, w_arc, h_arc, startAngle, sweepAngle);

                    int p_x_or = p1_or.X * bmp.Width / commom.width;
                    int p_y_or = p1_or.Y * bmp.Height / commom.height;
                    int p2_x_or = p2_or.X * bmp.Width / commom.width;
                    int p2_y_or = p2_or.Y * bmp.Height / commom.height;
                    Point p1_l_u_o = new Point(p_x_or, p_y_or);
                    Point p1_r_u_o = new Point(p2_x_or, p_y_or);
                    Point p1_r_b_o = new Point(p2_x_or, p2_y_or);
                    Point p1_l_b_o = new Point(p_x_or, p2_y_or);

                    g.DrawArc(red, p1_l_u_o.X, p1_l_u_o.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(red, p1_r_u_o.X, p1_r_u_o.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(red, p1_l_b_o.X, p1_l_b_o.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(red, p1_r_b_o.X, p1_r_b_o.Y, w_arc, h_arc, startAngle, sweepAngle);

                    int p_x_39_or = p1_39_or.X * bmp.Width / commom.width;
                    int p_y_39_or = p1_39_or.Y * bmp.Height / commom.height;
                    int p2_x_39_or = p2_39_or.X * bmp.Width / commom.width;
                    int p2_y_39_or = p2_39_or.Y * bmp.Height / commom.height;
                    Point p1_l_u_39_o = new Point(p_x_39_or, p_y_39_or);
                    Point p1_r_u_39_o = new Point(p2_x_39_or, p_y_39_or);
                    Point p1_r_b_39_o = new Point(p2_x_39_or, p2_y_39_or);
                    Point p1_l_b_39_o = new Point(p_x_39_or, p2_y_39_or);

                    g.DrawArc(green, p1_l_u_39_o.X, p1_l_u_39_o.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(green, p1_r_u_39_o.X, p1_r_u_39_o.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(green, p1_l_b_39_o.X, p1_l_b_39_o.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(green, p1_r_b_39_o.X, p1_r_b_39_o.Y, w_arc, h_arc, startAngle, sweepAngle);

                    int p_x_39_dig = p1_39_dig.X * bmp.Width / commom.width;
                    int p_y_39_dig = p1_39_dig.Y * bmp.Height / commom.height;
                    int p2_x_39_dig = p2_39_dig.X * bmp.Width / commom.width;
                    int p2_y_39_dig = p2_39_dig.Y * bmp.Height / commom.height;
                    Point p1_l_u_39_dig = new Point(p_x_39_dig, p_y_39_dig);
                    Point p1_r_u_39_dig = new Point(p2_x_39_dig, p_y_39_dig);
                    Point p1_r_b_39_dig = new Point(p2_x_39_dig, p2_y_39_dig);
                    Point p1_l_b_39_dig = new Point(p_x_39_dig, p2_y_39_dig);

                    g.DrawArc(pink, p1_l_u_39_dig.X, p1_l_u_39_dig.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(pink, p1_r_u_39_dig.X, p1_r_u_39_dig.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(pink, p1_l_b_39_dig.X, p1_l_b_39_dig.Y, w_arc, h_arc, startAngle, sweepAngle);
                    g.DrawArc(pink, p1_r_b_39_dig.X, p1_r_b_39_dig.Y, w_arc, h_arc, startAngle, sweepAngle);

                    g.Dispose();
                }
                string[] filename = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
                bmp.Save(filename[0] + "_pos_barcode.png", System.Drawing.Imaging.ImageFormat.Png);
                bmp.Dispose();
            }
        }

        private void Calculate_differences_barcode(SQL_connection sql)
        {
            string barcode_pos = sql.Get_Positions_Barcode(id_doc);
            string[] val_barcode_pos = barcode_pos.Split(':');
            p1_or = new Point(int.Parse(val_barcode_pos[0]), int.Parse(val_barcode_pos[1]));
            p2_or = new Point(int.Parse(val_barcode_pos[2]), int.Parse(val_barcode_pos[3]));
            p1_39_or = new Point(int.Parse(val_barcode_pos[4]), int.Parse(val_barcode_pos[5]));
            p2_39_or = new Point(int.Parse(val_barcode_pos[6]), int.Parse(val_barcode_pos[7]));

            barcode128_or = $"{p1_or.X}:{p1_or.Y}:{p2_or.X}:{p2_or.Y}";
            barcode39_or = $"{p1_39_or.X}:{p1_39_or.Y}:{p2_39_or.X}:{p2_39_or.Y}";
         
            string res_barcode = commom.Return_PositionBarcode(img_file);
            string[] res_barcode_pos = res_barcode.Split(':');
            p1_dig = new Point(int.Parse(res_barcode_pos[0]), int.Parse(res_barcode_pos[1]));
            p2_dig = new Point(int.Parse(res_barcode_pos[2]), int.Parse(res_barcode_pos[3]));
            p1_39_dig = new Point(int.Parse(res_barcode_pos[4]), int.Parse(res_barcode_pos[5]));
            p2_39_dig = new Point(int.Parse(res_barcode_pos[6]), int.Parse(res_barcode_pos[7]));

            int x_diff_or = p2_or.X - p1_or.X;
            int y_diff_or = p2_or.Y - p1_or.Y;

            int x_39_diff_or = p2_39_or.X - p1_39_or.X;

            // calcular a escala do ficheiro com base na primeira posição do y do código 39 
            // como as dimensões do ficheiro diminuiem é necessário também calcular a altura do código de barras atual
            // sabe-se que por defeito a altura do código de barras é 15 e comprimento 246
            scale_doc = Math.Round(Convert.ToDecimal(((double)p1_dig.Y / 842)), 2);
            scale_doc += 0.03m;

            if (scale_doc == 0.99m)
                scale_doc = 1.00m;

            //Console.WriteLine($"calculate scale {scale_doc}");

            if (scale_doc != 1.00m && !file_name.Contains("scan"))
            {
                int x_scale = Convert.ToInt16(x_diff_or * scale_doc);
                int x_39_scale = Convert.ToInt16(x_39_diff_or * scale_doc);
                int y_scale = Convert.ToInt16(y_diff_or * scale_doc);

                p1_dig.X += 2;
                p2_dig.X = Convert.ToInt16(p1_dig.X + x_scale);
                p2_dig.Y = Convert.ToInt16(p2_dig.Y + y_scale - y_diff_or);
                p2_39_dig.X = Convert.ToInt16(p1_39_dig.X + x_39_scale);
                p2_39_dig.Y = Convert.ToInt16(p2_39_dig.Y + y_scale - y_diff_or + 1);
            }

            int x_diff_dig = p2_dig.X - p1_dig.X;

            delta_y_or = p2_or.Y - p1_39_or.Y;
            delta_y_dig = p2_dig.Y - p1_39_dig.Y;

            barcode128_dig = $"{p1_dig.X}:{p1_dig.Y}:{p2_dig.X}:{p2_dig.Y}";
            barcode39_dig = $"{p1_39_dig.X}:{p1_39_dig.Y}:{p2_39_dig.X}:{p2_39_dig.Y}";

            int diff_y = delta_y_dig - delta_y_or;
            int diff_x = x_diff_dig - x_diff_or;
            if (diff_y <= -44)
                diff_y = -40;
            if (diff_x == -14)
                diff_x = -7;

            diff_barcode = new Point(diff_x, diff_y);
            prop = new PointF((float)x_diff_dig / x_diff_or, (float)delta_y_dig / delta_y_or);
        }


        private void Calculate_differences_document(SQL_connection sql, Commom commom)
        {            
            dimensions = sql.GetDimensionsDoc_db(id_doc);
            string[] val_dim = dimensions.Split(':');
            doc_dim_or = new Point(int.Parse(val_dim[0]), int.Parse(val_dim[1]));
            image_dim_or = new Point(int.Parse(val_dim[2]), int.Parse(val_dim[3]));

            commom.GetDimensionsDocument(file_name);
            commom.GetDimensionsImage(img_file);
            doc_dim_dig = new Point(commom.width, commom.height);
            image_dim_dig = new Point(commom.width_bmp, commom.height_bmp);

            scan_dimen = $"{doc_dim_dig.X}:{doc_dim_dig.Y}:{image_dim_dig.X}:{image_dim_dig.Y}";

            diff_doc_dim = new Point(doc_dim_or.X - doc_dim_dig.X, doc_dim_or.Y - doc_dim_dig.Y);
            diff_img_dim = new Point(image_dim_or.X - image_dim_dig.X, image_dim_or.Y - image_dim_dig.Y);

            diff_dimen = $"{diff_doc_dim.X}:{diff_doc_dim.Y}";
            diff_bmp_dim = $"{diff_img_dim.X}:{diff_img_dim.Y}";
        }

        private void Print_values()
        {
            Console.WriteLine($"Original dimensions {dimensions}, Scan dimensions {scan_dimen}, diff doc {diff_dimen}, bmp {diff_bmp_dim}");
            Console.WriteLine($"Original pos barcode 128 {barcode128_or} | barcode 39 {barcode39_or}");
            Console.WriteLine($"Digital pos barcode 128 {barcode128_dig} | barcode 39 {barcode39_dig}");
            Console.WriteLine("--- Difference between original and digital");
            Console.WriteLine($"Differences X = {diff_barcode.X}, Y = {diff_barcode.Y}");
            Console.WriteLine($"Prop x {prop.X}, y {prop.Y}");
            Console.WriteLine($"Delta original {delta_y_or}, delta dig {delta_y_dig}");
        }


        private void Retificate_Load(object sender, EventArgs e)
        {
            SQL_connection sql = new SQL_connection();
            if (!String.IsNullOrEmpty(result_barcode))
            {
                string[] resultado = result_barcode.Split(';');
                id_doc = int.Parse(resultado[0]);

                string res_doc = sql.Search_document(id_doc);
                if (String.IsNullOrEmpty(res_doc))
                {
                    MessageBox.Show(errorFileDatabase);
                    Close();
                    Dispose();
                }
                else
                {
                    Calculate_differences_barcode(sql);
                    Calculate_differences_document(sql, commom);
                    //Draw_Points_Barcodes(); // descomentar caso pretender visualizar os pontos
                    Print_values();
                    string[] col_sql = res_doc.Split(';');
                    dct_name.Text = col_sql[0];
                    user.Text = col_sql[1];
                    sigla.Text = col_sql[2];
                    posto.Text = col_sql[3];
                    Controls.Add(dct_name);
                    Controls.Add(user);
                    Controls.Add(sigla);
                    Controls.Add(posto);
                    Show();
                }
            }
        }


        private void Forense_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(infoAnaliseForense);
            SQL_connection sql = new SQL_connection();
            commom.RetificateAnalise(id_doc, sql, file_name, diff_barcode.X, diff_barcode.Y, prop.X, prop.Y, diff_doc_dim.X, diff_doc_dim.Y, diff_img_dim.X, diff_img_dim.Y, (double) scale_doc);
        }

        private void Retificate_FormClosed(object sender, FormClosedEventArgs e)
        {
            string name_file = commom.Get_file_name_using_split(file_name);
            string file_rotated = name_file + "_rotated.png";
            if (File.Exists(file_rotated))
                File.Delete(file_rotated);  
            if (File.Exists(img_file))
                File.Delete(img_file);
        }
    }
}
