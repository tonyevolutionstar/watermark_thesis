using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WatermarkApp
{
    /// <summary>
    /// Função auxiliar para operaçoes no ficheiro
    /// </summary>
    public class AuxFunc
    {
        private string[] filename;
        private int id_doc;
        private SQL_connection sql;
        private int size_qrcode;
        private int h;
        private int w;
        private Dictionary<string, Point> qrcode_points;
        private List<string> combs;
        private string pngfile;


        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="id_doc"></param>
        /// <param name="sql"></param>
        /// <param name="filename"></param>
        /// <param name="size_qrcode"></param>
        /// 
        public AuxFunc(int id_doc, SQL_connection sql, string filename, int size_qrcode)
        {
            this.id_doc = id_doc;
            this.sql = sql;
            this.size_qrcode = size_qrcode;
            using (Stream inputPdfStream = new FileStream(filename , FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true;
                w = (int)reader.GetPageSize(1).Width;
                h = (int)reader.GetPageSize(1).Height;
                reader.Close();
                reader.Dispose();
            }   
            this.pngfile = Convert_pdf_png(filename);
        }

        /// <summary>
        /// Converte o ficheiro pdf para png
        /// </summary>
        /// <param name="f">Nome do ficheiro</param>
        /// <returns>Nome do ficheiro + ".png"</returns>
        public string Convert_pdf_png(string f)
        {
            var dd = System.IO.File.ReadAllBytes(f);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            string[] filename = f.Split(new[] { ".pdf" }, StringSplitOptions.None);
            System.IO.File.WriteAllBytes(filename[0] + ".png", pngByte);
            return filename[0] + ".png";
        }

        /// <summary>
        /// Calcula a interseção
        /// </summary>
        /// <param name="position"></param>
        /// <param name="qrcode_file"></param>
        public void CalculateIntersection(string position, string qrcode_file)
        {
            string f = Convert_pdf_png(qrcode_file);
            Bitmap bmp = new Bitmap(f);
            Bitmap getcolors = new Bitmap(pngfile);

            qrcode_points = FillDictionary(position, bmp);
       
            string qrcode_comb;
            combs = new List<string>();
      
            foreach (KeyValuePair<string, Point> entry in qrcode_points)
            {
                string[] val0 = entry.Key.Split('_');
                foreach (KeyValuePair<string, Point> entry2 in qrcode_points)
                {
                    string[] val1 = entry2.Key.Split('_');
                    if (!val0[0].Equals(val1[0]))
                    {
                        qrcode_comb = entry.Key + ":" + entry2.Key;
                        if (!combs.Contains(entry2.Key + ":" + entry.Key))
                            combs.Add(qrcode_comb);
                    }
                }
            }

            // get rects without the origin point be the same
            for(int i = 0; i < combs.Count; i ++)
            {
                string[] points_i = combs[i].Split(':');
                string[] side_qrcode_i = points_i[0].Split('_');
                for (int j = 0; j < combs.Count; j ++)
                {
                    string[] points_j = combs[j].Split(':');
                    string[] side_qrcode_j = points_j[0].Split('_');
                    if (!side_qrcode_i[0].Equals(side_qrcode_j[0]))
                    {
                        qrcode_points.TryGetValue(points_i[0], out Point A);
                        qrcode_points.TryGetValue(points_i[1], out Point B);
                        qrcode_points.TryGetValue(points_j[0], out Point C);
                        qrcode_points.TryGetValue(points_j[1], out Point D);
                        Point res = Intersection(A, B, C, D);
                      
                        if ((res.X > 0 && res.X < bmp.Width) && res.Y > 0 && res.Y < bmp.Height && res.X != 0 && res.Y != 0 && (res.X != A.X && res.Y != A.Y) && (res.X != B.X && res.Y != B.Y) && (res.X != C.X && res.Y != C.Y) && (res.X != D.X && res.Y != D.Y))
                        {
                            string ch = Get_Value_in(getcolors, res.X, res.Y);
                            if (!String.IsNullOrEmpty(ch) && !ch.Equals("") && !String.IsNullOrWhiteSpace(ch))
                            {
                                string line1_points = A.X + "," + A.Y + ":" + B.X + "," + B.Y;
                                string line2_points = C.X + "," + C.Y + ":" + D.X + "," + D.Y;
                                string inter_point = res.X + "," + res.Y;

                                sql.Insert_forense_analises(id_doc, combs[i], combs[j], inter_point, ch, line1_points, line2_points);
                            }
                        }
                    }
                }
            }

            bmp.Dispose();
            getcolors.Dispose();        
        }

        /// <summary>
        /// Preenche os qrcodes de 1 a 9 com as respetivas posições
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bmp"></param>
        /// <returns>Dicionario com as posições</returns>
        private Dictionary<string, Point> FillDictionary(string position, Bitmap bmp)
        {
            Dictionary<string, Point> qrcode_points = new Dictionary<string, Point>();
            for (int i = 0; i < 9; i++)
            {
                string[] pos_qrcodes = position.Split('|');
                string[] qrcode = pos_qrcodes[i].Split(',');
                
                int x_qrcode = int.Parse(qrcode[0]) * bmp.Width / w ;
                int y_qrcode = int.Parse(qrcode[1]) * bmp.Height / h;
                Point qrcode_l = new Point(x_qrcode + Convert.ToInt32(size_qrcode/2) + 5, y_qrcode - size_qrcode * 2 - size_qrcode - 50);
                Point qrcode_r = new Point(x_qrcode + size_qrcode * 3 + Convert.ToInt32(size_qrcode / 2), y_qrcode - size_qrcode * 2 - size_qrcode - 50); 
                Point qrcode_b = new Point(x_qrcode + Convert.ToInt32(size_qrcode / 2), y_qrcode - size_qrcode + Convert.ToInt32(size_qrcode / 2)); 
                qrcode_points.Add("qrcode" + (i + 1) + "_l", qrcode_l);
                qrcode_points.Add("qrcode" + (i + 1) + "_r", qrcode_r);
                qrcode_points.Add("qrcode" + (i + 1) + "_b", qrcode_b);
            }

            return qrcode_points;
        }

        /// <summary>
        /// Calcula a interseção entre duas retas
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns>Ponto de interseção</returns>
        private Point Intersection(Point A, Point B, Point C, Point D)
        {
            int x1 = A.X;
            int x2 = B.X;
            int x3 = C.X;
            int x4 = D.X;

            int y1 = A.Y;
            int y2 = B.Y;
            int y3 = C.Y;
            int y4 = D.Y;
            double t = (double) ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
            double u = (double)((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            Point inter = new Point();
            if ((t >= 0 && t <= 1) && (u >= 0 && u <= 1))
            {
                double x = x3 + u * (x4 - x3);
                double y = y3 + u * (y4 - y3);

                inter = new Point((int)x, (int)y);
            }

            return inter;
        }

        /// <summary>
        /// Obtem a letra próxima ao valor da interseção
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Letra</returns>
        private string Get_Value_in(Bitmap bmp, int x, int y)
        {
            string ch = "";
            List<string> pos_char = sql.Get_characters_Pos(id_doc);

            foreach (string values in pos_char)
            {
                string[] val = values.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries); ;
                string[] pos_val = val[1].Split(',');
                int new_x = Convert.ToInt32(int.Parse(pos_val[0]) * bmp.Width / w);
                int new_y = Convert.ToInt32(int.Parse(pos_val[1]) * bmp.Height / h);
                int final_x = Convert.ToInt32(int.Parse(pos_val[2]) * bmp.Width / w);
                int final_y = Convert.ToInt32(int.Parse(pos_val[3]) * bmp.Height / h);

                // verifica se o valor da interseção está proximo a uma letra
                if (x >= new_x && x <= final_x && y >= new_y && y <= final_y)
                {
                    return val[0]; 
                }
            }
            return ch;
        }

        /// <summary>
        /// Usado pela Analise Forense
        /// </summary>
        /// <param name="return_list"></param>
        /// <param name="qrcode_file"></param>
        public string DrawImage(List<string> return_list, string qrcode_file)
        {
            string f = Convert_pdf_png(qrcode_file);
            Bitmap bmp = new Bitmap(f);
            Graphics g = Graphics.FromImage(bmp);
            Pen yellow = new Pen(Color.Yellow, 3);
            int width = 15;
            int height = 20;
            int startAngle = 0;
            int sweepAngle = 360;

            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Blue);

            //inter_point + "|" + inter_char + "|" + line1_points + "|" + line2_points
            for (int i = 0; i < return_list.Count; i++)
            {
                string[] values = return_list[i].Split('|');
                string[] inter_point = values[0].Split(',');
                string ch = values[1];

                int res_x = Convert.ToInt32(inter_point[0]);
                int res_y = Convert.ToInt32(inter_point[1]);
                Point res = new Point(res_x, res_y);

                g.DrawString(ch, drawFont, drawBrush, res);
                g.DrawArc(yellow, res.X, res.Y, width, height, startAngle, sweepAngle);          
            }

            filename = f.Split(new[] { ".png" }, StringSplitOptions.None);
            bmp.Save(filename[0] + "_line.png");
            bmp.Dispose();
            return filename[0] + "_line.png";
        }


        /*
         * 
         * 
         * 
            private Point Intersect(Point A, Point B, Point C, Point D)
        {
            int a1 = B.Y - A.Y;
            int b1 = A.X - B.X;
            int c1 = a1 * A.X + b1 * A.Y;

            int a2 = D.Y - C.Y;
            int b2 = C.X - D.X;
            int c2 = a2 * C.X + b2 * C.Y;

            int determinant = a1 * b2 - a2 * b1;

            if (determinant != 0)
            {
                double x = (double) (b2 * c1 - b1 * c2) / determinant;
                double y = (double) (a1 * c2 - a2 * c1) / determinant;
                x = Math.Abs(x);
                y = Math.Abs(y);
                return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                
            }
            return new Point(0, 0);
        }

        
        private void an_line(Bitmap bmp, Point A, Point B, Point C, Point D)
        {
            Graphics g = Graphics.FromImage(bmp);
            int x1 = A.X;
            int x2 = B.X;
            int x3 = C.X;
            int x4 = D.X;

            int y1 = A.Y;
            int y2 = B.Y;
            int y3 = C.Y;
            int y4 = D.Y;


            if ((x2 - x1) != 0)
            {
                int b1 = Convert.ToInt32((y2 - y1) / (x2 - x1));
                if ((x4 - x3) != 0)
                {
                    int b2 = Convert.ToInt32((y4 - y3) / (x4 - x3));

                    int c1 = y2 - b1 * x2;
                    int c2 = y4 - b2 * x4;

                    int y_eq1 = b1 * x1 + c1;
                    int y_eq2 = b2 * x2 + c2;
                    if (y_eq1 != y_eq2)
                    {
                        if (b1 != 0)
                        {

                            int x = Convert.ToInt32((y2 - c1) / b1);
                            int y = y1;
                            Console.WriteLine("Intersection: " + x + "," + y);
                            //bmp.SetPixel(x, y, Color.Yellow);
                            // Create pen.
                            Pen yellow = new Pen(Color.Yellow, 3);

                            // Create coordinates of the rectangle
                            // to the bound ellipse.
                          
                            int width = 100;
                            int height = 200;

                            // Create start and sweep
                            // angles on ellipse.
                            int startAngle = 0;
                            int sweepAngle = 360;

                            // Draw arc to screen.
                            g.DrawArc(yellow, x, y, width,
                                     height, startAngle, sweepAngle);
                        }
                    }
                }
            }
        }


        private void line_eq(Bitmap bmp, Point A, Point B, Point C, Point D)
        {
            
            if ((B.X - A.X) > 0 )
            {
                int m1 = Convert.ToInt32((B.Y - A.Y) / (B.X - A.X));
                if ((D.X - C.X) > 0)
                {
                    int m2 = Convert.ToInt32((D.Y - C.Y) / (D.X - C.X));
                    // y - y1 = m(x-x1) <=> y = m(x-x1) + y1 <=> y = mx - mx1 + y1
                    int y1_t = -m1 * A.X + A.Y;
                    int y2_t = -m2 * B.X + B.Y;
                    if ((m1 - m2) > 0)
                    {
                        int x = Math.Abs(Convert.ToInt32((y2_t - y1_t) / (m1 - m2)));
                        int y = Math.Abs(m1 * x + m2);
                        if (x< bmp.Width && y< bmp.Height)
                            bmp.SetPixel(x, y, Color.Blue);
                    }
                    
                }
                
            }
        }
         private Point a_int(Point A, Point B, Point C, Point D)
        {
            int x1 = A.X;
            int x2 = B.X;
            int x3 = C.X;
            int x4 = D.X;

            int y1 = A.Y;
            int y2 = B.Y;
            int y3 = C.Y;
            int y4 = D.Y;

            int t1 = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4);
            int t2 = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            int u1 = (x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2);
            int u2 = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (t2 != 0 && u2 != 0)
            {
                double t = (double)t1 / t2;
                double u = (double)u1 / u2;

                if (0.0 >= t && t <= 1.0 && 0.0 >= u && u <= 1.0)
                {
                    int x = Math.Abs(Convert.ToInt32(x3 + u * (x4 - x3)));
                    int y = Math.Abs(Convert.ToInt32(y3 + u * (y4 - y3)));
                    return new Point(x, y);
                }
            }
            return new Point(0, 0);

        }



        private void int_wiki(Bitmap bmp, Point A, Point B, Point C, Point D)
        {
            int x1 = A.X;
            int x2 = B.X;
            int x3 = C.X;
            int x4 = D.X;

            int y1 = A.Y;
            int y2 = B.Y;
            int y3 = C.Y;
            int y4 = D.Y;

            int px1 = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
            int px2 = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            int py1 = (x1 * y2 - y1 * x2)  * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
          

            if (px2 > 0)
            {
                int px = Math.Abs(Convert.ToInt32(px1 / px2));
                int py = Math.Abs(Convert.ToInt32(py1 / px2));
                if (px < bmp.Width && py < bmp.Height)
                    bmp.SetPixel(px, py, Color.Violet);
            }

        }
 

      


        private void another_int(Bitmap bmp, Point A, Point B, Point C, Point D)
        {
       
            if ((B.X - C.X) * (B.Y - A.Y) != 0 )
            {
                double d = (D.Y - C.Y) * (B.X - A.X) - (B.Y - A.Y) * (D.X - C.X);
                if (d == 0)
                    Console.WriteLine("Paralel");
                else
                {
                    int u = Convert.ToInt32(((C.X - A.X) * (D.Y - C.Y) - (C.Y - A.Y) * (D.X - C.X)) / d);
                    int v = Convert.ToInt32(((C.X - A.X) * (B.Y - A.Y) - (C.Y - A.Y) * (B.X - A.X)) / d);

                    // The fractional point will be between 0 and 1 inclusive if the lines
                    // intersect.  If the fractional calculation is larger than 1 or smaller
                    // than 0 the lines would need to be longer to intersect.
                    if (u >= 0d && u <= 1d && v >= 0d && v <= 1d)
                    {
                        int x = A.X + (u * (B.X - A.X));
                        int y = A.Y + (u * (B.Y - A.Y));

                        Console.WriteLine("Point intersection: " + x + "," + y);
                        bmp.SetPixel(x, y, Color.Blue);
                    }
                }
            }
        }

         private string point_intersection(Bitmap bmp, Point A, Point B, Point C, Point D)
        {
            Graphics g = Graphics.FromImage(bmp);
            int a1 = B.Y - A.Y;
            int b1 = A.X - B.X;
            int c1 = a1 * A.X + b1 * A.Y;

            int a2 = D.Y - C.Y;
            int b2 = C.X - D.X;
            int c2 = a2 * C.X + b2 * C.Y;

            int determinant = a1 * b2 - a2 * b1;

            if (determinant != 0)
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                x = Math.Abs(x);
                y = Math.Abs(y);

                if (x < bmp.Width && y < bmp.Height && x != A.X && y!= A.Y && x != B.X && y != B.Y && x != C.X && y != C.Y && x != D.X && y != D.Y)
                {
                    Pen yellow = new Pen(Color.Yellow, 3);

                    int width = 25;
                    int height = 50;
                    int startAngle = 0;
                    int sweepAngle = 360;
                    //bmp.SetPixel(Convert.ToInt32(x) , Convert.ToInt32(y) , Color.Red);
                    // Draw arc to screen.
                    //g.DrawArc(yellow, Convert.ToInt32(x), Convert.ToInt32(y), width,height, startAngle, sweepAngle);
                    return Convert.ToInt32(x) + "," + Convert.ToInt32(y);
                }        
                else return "0";
            }
            return "0";
        }
        */


    }
}
