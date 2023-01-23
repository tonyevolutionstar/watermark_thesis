using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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


        public string Convert_pdf_png(string f)
        {
            var dd = System.IO.File.ReadAllBytes(f);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            string[] filename = f.Split(new[] { ".pdf" }, StringSplitOptions.None);
            System.IO.File.WriteAllBytes(filename[0] + ".png", pngByte);
            return filename[0] + ".png";
        }

        private bool CalculateDifferenceBetweenTwoPoints(Point p1, Point p2)
        {
            int a1 = p1.X;
            int a2 = p1.Y;
            int b1 = p2.X;
            int b2 = p2.Y;

            int dif_X = (a1 - b1);
            int dif_Y = (a2 - b2);

            if (dif_X > 0 && dif_X < 5)
            {
                return true;
            }
            else if (dif_Y > 0 && dif_Y < 5)
            {
                return true;
            }
            return false;

        }
     
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
     
            Console.WriteLine("Rects without duplicates " + combs.Count);

            int without_p = 1;
            List<string> p = new List<string>();

            Dictionary<string,string> analiseDic = new Dictionary<string, string>();

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
                        without_p++;
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
                                string delimitador = "|";

                                p.Add(id_doc.ToString() + delimitador + combs[i].ToString() + delimitador + combs[j].ToString() + delimitador + inter_point.ToString() + delimitador + ch + delimitador + line1_points + delimitador + line2_points);

                                sql.Insert_forense_analises(id_doc, combs[i], combs[j], inter_point, ch, line1_points, line2_points);
                            }
                        }
                    }
                }
            }

            sql.RemoveDuplicatesAnaliseForenseLine1(id_doc);
            sql.RemoveDuplicatesAnaliseForenseLine2(id_doc);

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


        private bool verify_Value_in(Bitmap bmp, int x, int y)
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
                    return true;
                }
            }
            return false;
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

            for (int i = 0; i < return_list.Count; i++)
            {
                string[] values = return_list[i].Split('|');
                string[] inter_point = values[0].Split(',');
                string ch = values[1];

                int res_x = Convert.ToInt32(inter_point[0]);
                int res_y = Convert.ToInt32(inter_point[1]);
                Point intersection = new Point(res_x, res_y);

                g.DrawString(ch, drawFont, drawBrush, intersection);
                g.DrawArc(yellow, intersection.X, intersection.Y, width, height, startAngle, sweepAngle);          
            }

            filename = f.Split(new[] { ".png" }, StringSplitOptions.None);
            bmp.Save(filename[0] + "_line.png");
            bmp.Dispose();
            return filename[0] + "_line.png";
        }


        private bool verifyAnaliseForense(string ch, int x, int y, Bitmap bmp)
        {
            List<string> pos_char = sql.Get_characters_Pos(id_doc);
            int count = 1;

            foreach (string values in pos_char)
            {
                string[] val = values.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries); ;
                string[] pos_val = val[1].Split(',');
                int new_x = Convert.ToInt32(int.Parse(pos_val[0]) * bmp.Width / w);
                int new_y = Convert.ToInt32(int.Parse(pos_val[1]) * bmp.Height / h);
                int final_x = Convert.ToInt32(int.Parse(pos_val[2]) * bmp.Width / w);
                int final_y = Convert.ToInt32(int.Parse(pos_val[3]) * bmp.Height / h);


                if (val[0].Equals(ch))
                {
                    if (x >= new_x && x <= final_x && y >= new_y && y <= final_y)
                    {
                    }
                }
            }

    

            return false;
        }
    }
}
