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
        private string file_name;
        private int size_qrcode;
        private int h;
        private int w;
        private Dictionary<string, Point> qrcode_points;
        private List<string> combs;
        private string[] characters;
        private Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
        private List<string> all_results = new List<string>();

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="size_qrcode"></param>
        /// <param name="ch"></param>
        public AuxFunc(string filename, int size_qrcode, string[] ch)
        {
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
            this.characters = ch;
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
        /// desenha linhas
        /// </summary>
        /// <param name="position"></param>
        /// <param name="qrcode_file"></param>
        public void DrawLines(string position,string qrcode_file)
        {
            string f = Convert_pdf_png(qrcode_file);
            Bitmap bmp = new Bitmap(f);
            Graphics g = Graphics.FromImage(bmp);
            Pen greenPen = new Pen(Color.Green, 3);
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

            for (int i = 0; i < combs.Count; i++)
            {
                string[] points = combs[i].Split(':');
                qrcode_points.TryGetValue(points[0], out Point p1);
                qrcode_points.TryGetValue(points[1], out Point p2);
                g.DrawLine(greenPen, p1, p2);
                Console.WriteLine("ret " + combs[i] + " point " + p1 + "," + p2);
                //Ddaline(combs[i], p1, p2, g, bmp);
            }

            string[] filename = f.Split(new[] { ".png" }, StringSplitOptions.None);
            bmp.Save(filename[0] + "_line.png");
            bmp.Dispose();
        }

        private Dictionary<string, Point> FillDictionary(string position, Bitmap bmp)
        {
            Console.WriteLine("size_qrcode: " + this.size_qrcode);
            Dictionary<string, Point> qrcode_points = new Dictionary<string, Point>();
            for (int i = 0; i < 9; i++)
            {
                string[] pos_qrcodes = position.Split('|');
                string[] qrcode = pos_qrcodes[i].Split(',');
                int x_qrcode = int.Parse(qrcode[0]) ;
                int y_qrcode = int.Parse(qrcode[1]) ;
                Point qrcode_l = new Point(x_qrcode + this.size_qrcode, Math.Abs(this.size_qrcode - y_qrcode));
                Point qrcode_r = new Point(x_qrcode + this.size_qrcode * 3, Math.Abs(this.size_qrcode - y_qrcode));
                Point qrcode_b = new Point(x_qrcode + this.size_qrcode, Math.Abs((this.size_qrcode * 3) - y_qrcode));
                qrcode_points.Add("qrcode" + (i + 1) + "_l", qrcode_l);
                qrcode_points.Add("qrcode" + (i + 1) + "_r", qrcode_r);
                qrcode_points.Add("qrcode" + (i + 1) + "_b", qrcode_b);
            }

            return qrcode_points;
        }

        private void AddDictionary(string rect, string res)
        {
            if (!results.ContainsKey(rect))
            {
                all_results.Add(res);
                results.Add(rect, all_results);
            }
            else
            {
                all_results.Add(res);
                results[rect].Add(res);
            }
        }

        private void Verify_value(Dictionary<string, List<Point>> points_rets, Dictionary<string, Point> points_char)
        {
            int range = 5;
            Dictionary<string, List<Point>> mem_result = new Dictionary<string, List<Point>>();


            foreach (KeyValuePair<string, List<Point>> rect in points_rets)
            {
                //Console.WriteLine(rect.Key);
                for(int i = 0; i < rect.Value.Count; i ++)
                {
                    //Console.WriteLine("val " + rect.Value[i]);
                    List<string> characters = new List<string>();
                    foreach (KeyValuePair<string, Point> c in points_char)
                    {
                        string res = c.Key;

                        if ((rect.Value[i].X - range) > c.Value.X && c.Value.X < (rect.Value[i].X + range) && (rect.Value[i].Y - range) > c.Value.Y && c.Value.Y < (rect.Value[i].Y + range))
                        {
                            characters.Add(res);
                        }
                    }
                    //Console.WriteLine("characters size " + characters.Count);
                }
            }
        }

        private void Ddaline(string ret, Point p1, Point p2, Graphics g, Bitmap bmp)
        {
            List<Point> points;
            Dictionary<string, List<Point>> points_rets = new Dictionary<string, List<Point>>();
            Dictionary<string, Point> points_char = new Dictionary<string, Point>();

            double xinc, yinc, x, x1, y, y1, steps;
            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;
            if (deltaX > deltaY) steps = deltaX;
            else steps = deltaY;

            xinc = deltaX / steps;
            yinc = deltaY / steps;
            x = x1 = p1.X;
            y = y1 = p1.Y;

            foreach (string c in characters)
            {
                string[] val = c.Split('|');
                string[] coord = val[1].Split(',');
                points_char.Add(c, new Point(int.Parse(coord[0]), int.Parse(coord[1])));   
            }

            for (int k = 1; k <= steps; k++)
            {
                x = x + xinc;
                x = Math.Round(x, 0);
                y = y + yinc;
                y = Math.Round(y, 0);
                if(points_rets.ContainsKey(ret))
                {
                    Point p = new Point((int)x, (int)y);
                    //points.Add(p);
                    points_rets[ret].Add(p);
                }
                else
                {
                    Point p = new Point((int)x, (int)y);
                   
                    points_rets.Add(ret, new List<Point>() { p });
                }
            }

            Verify_value(points_rets, points_char);
        }
    }
}
