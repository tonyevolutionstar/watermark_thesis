using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace WatermarkApp
{
    /// <summary>
    /// Função auxiliar para operaçoes no ficheiro
    /// </summary>
    public class AuxFunc
    {
        private string[] filename;
        private int size_qrcode;
        private int h;
        private int w;
        private Dictionary<string, Point> qrcode_points;
        private List<string> combs;
        private string[] characters;

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
                //Ddaline(combs[i], p1, p2, g, bmp);
            }

            filename = f.Split(new[] { ".png" }, StringSplitOptions.None);
            bmp.Save(filename[0] + "_line.png");
            bmp.Dispose();
            
        }

        private Dictionary<string, Point> FillDictionary(string position, Bitmap bmp)
        {
            Dictionary<string, Point> qrcode_points = new Dictionary<string, Point>();
            for (int i = 0; i < 9; i++)
            {
                string[] pos_qrcodes = position.Split('|');
                string[] qrcode = pos_qrcodes[i].Split(',');
                int x_qrcode = int.Parse(qrcode[0]) * bmp.Width / w ;
                int y_qrcode = int.Parse(qrcode[1]) * bmp.Height / h;
                Point qrcode_l = new Point(x_qrcode + size_qrcode, y_qrcode - size_qrcode * 3);
                Point qrcode_r = new Point(x_qrcode + size_qrcode * 3, y_qrcode - size_qrcode * 3);
                Point qrcode_b = new Point(x_qrcode +  size_qrcode, y_qrcode - size_qrcode);
                qrcode_points.Add("qrcode" + (i + 1) + "_l", qrcode_l);
                qrcode_points.Add("qrcode" + (i + 1) + "_r", qrcode_r);
                qrcode_points.Add("qrcode" + (i + 1) + "_b", qrcode_b);
            }

            return qrcode_points;
        }

   
        public void draw_point(string f, string[] characters)
        {
            Bitmap bmp = new Bitmap(f);

            foreach(string c in characters)
            {
                string[] val = c.Split('|');
                string[] pos = val[1].Split(',');
                int x = int.Parse(pos[0]);
                int y = int.Parse(pos[1]);

                bmp.SetPixel((int)x * bmp.Width/w, (int)y * bmp.Height/h, Color.Purple);
            }
            bmp.Save(filename[0] + "_auxchar.png");

            bmp.Dispose();
        }

        private Dictionary<string, List<string>> Verify_value(Dictionary<string, List<Point>> points_rets, Dictionary<string, Point> points_char)
        {
            //int range = 0;
            Dictionary<string, List<string>> mem_result = new Dictionary<string, List<string>>();
            List<string> characters;
            foreach (KeyValuePair<string, List<Point>> rect in points_rets)
            {
                for(int i = 0; i < rect.Value.Count; i ++)
                {
                    characters = new List<string>();
                    foreach (KeyValuePair<string, Point> c in points_char)
                    {
                        string res = c.Key;

                        if (rect.Value[i].X == c.Value.X && rect.Value[i].Y == c.Value.Y )
                            characters.Add(res);
                    }
                    if (!mem_result.ContainsKey(rect.Key))
                        mem_result.Add(rect.Key, characters);  
                }
            }
            return mem_result;
        }

        private void Ddaline(string ret, Point p1, Point p2, Graphics g, Bitmap bmp)
        {
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
                Point point = new Point((int)x, (int)y);
                //Console.WriteLine("point " + point);

                if (points_rets.ContainsKey(ret))
                {
                    Point p = new Point((int)x, (int)y);
                    points_rets[ret].Add(p);
                }
                else
                {
                    Point p = new Point((int)x, (int)y);
                    points_rets.Add(ret, new List<Point>() { p });
                }
            }

            Dictionary<string, List<string>> mem_result = Verify_value(points_rets, points_char);
            foreach (KeyValuePair <string,List<string>> m_res in mem_result)
            {
                Console.WriteLine(m_res.Key);
                for (int i = 0; i < m_res.Value.Count; i++)
                {
                    string[] val = m_res.Value[i].Split('|');
                    string[] coord = val[1].Split(',');
                    int x_c = int.Parse(coord[0]);
                    int y_c = int.Parse(coord[1]);
                    //bmp.SetPixel((int)x_c * bmp.Width / w, (int)y_c * bmp.Height / h, Color.Brown);
                    Console.WriteLine(m_res.Value[i]);
                }
            }
        }
    }
}
