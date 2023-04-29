using System;
using System.Drawing;
using Microsoft.VisualBasic.FileIO;

namespace ConsoleNet
{
    public class Retificate
    {
        //or - original - o documento que está na base de dados
        //dig - digital - o documento digatalizado que apresenta na aplicação
        private readonly string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private readonly string infoAnaliseForense = "Procedendo à Análise Forense, aguarde!";
        private readonly string error_readBarcode = "Não consegui ler o código de barras";

        private readonly Commom commom;
        private readonly string file_name;
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

        private readonly TrackerServices tracker = new TrackerServices();
        private string img_file;

        public Retificate(string file_name)
        {
            this.file_name = file_name;
            tracker.WriteFile("---------- Retificar -----------");
            tracker.WriteFile($"retificação do ficheiro {file_name} a iniciar");
            commom = new Commom();
            string name = commom.Get_file_name_using_split(file_name);
            img_file = name + ".png";
            result_barcode = commom.Read_barcode(img_file);
            if (result_barcode == commom.errorReadBarcode)
            {
                tracker.WriteFile($"retificação do ficheiro {file_name} falhou - {error_readBarcode}");
                Console.WriteLine(error_readBarcode);
            }
            Retificate_Load();
        }

        private void Retificate_Load()
        {
            SQL_connection sql = new SQL_connection();
            if (!String.IsNullOrEmpty(result_barcode))
            {
                string[] resultado = result_barcode.Split(';');
                id_doc = int.Parse(resultado[0]);
                string res_doc = sql.Search_document(id_doc);
                if (String.IsNullOrEmpty(res_doc))
                {
                    Console.WriteLine(errorFileDatabase);
                }
                else
                {
                    Calculate_differences_barcode(sql);
                    Calculate_differences_document(sql, commom);
                    //Draw_Points_Barcodes(); // descomentar caso pretender visualizar os pontos
                    //Print_values();
                    string[] col_sql = res_doc.Split(';');
                    string dct_name = col_sql[0];
                    string user = col_sql[1];
                    string sigla = col_sql[2];
                    string posto = col_sql[3];

                    Console.WriteLine($"Resultado leitura Nome do documento {dct_name}, utilizador {user}, sigla {sigla}, posto {posto}");
                    Forense();
                    Console.WriteLine("Acabei a análise forense");

                    string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
                    string img_file = s_doc[0] + ".png";
                    string integrity_img_file = s_doc[0] + "_integrity.png";

                    FileSystem.DeleteFile(img_file, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                    FileSystem.DeleteFile(integrity_img_file, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                }
            }
        }

        private void Forense()
        {
            Console.WriteLine(infoAnaliseForense);
            SQL_connection sql = new SQL_connection();
            commom.RetificateAnalise(id_doc, sql, file_name, diff_barcode.X, diff_barcode.Y, prop.X, prop.Y, diff_doc_dim.X, diff_doc_dim.Y, diff_img_dim.X, diff_img_dim.Y, (double)scale_doc);
        }


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

            Console.WriteLine($"calculate scale {scale_doc}");

            if (p1_39_dig.Y == 10)
                p1_39_dig.Y = 11;

            if(scale_doc != 1.00m)
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

            int height_or_doc = p2_or.Y - p1_39_or.Y;
            int height_dig_doc = p2_dig.Y - p1_39_dig.Y;

            barcode128_dig = $"{p1_dig.X}:{p1_dig.Y}:{p2_dig.X}:{p2_dig.Y}";
            barcode39_dig = $"{p1_39_dig.X}:{p1_39_dig.Y}:{p2_39_dig.X}:{p2_39_dig.Y}";

            diff_barcode = new Point(x_diff_dig - x_diff_or, height_dig_doc - height_or_doc);

            delta_y_or = p2_or.Y - p2_39_or.Y;
            delta_y_dig = p2_dig.Y - p2_39_dig.Y;

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
            Console.WriteLine($"Differences 128 X = {diff_barcode.X}, Y = {diff_barcode.Y}");
            Console.WriteLine($"Prop x {prop.X}, y {prop.Y}");
            Console.WriteLine($"Delta original {delta_y_or}, delta dig {delta_y_dig}");
        }
    }
}
