using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Text;

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
            characters = ch;
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
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string text_file_debug = partialPath + @"\Ficheiros\text_debug.txt";
            string f = Convert_pdf_png(qrcode_file);
            Bitmap bmp = new Bitmap(f);
            Graphics g = Graphics.FromImage(bmp);
            Pen greenPen = new Pen(Color.Green, 3);
            qrcode_points = FillDictionary(position, bmp);
            List<string> sb = new List<string>();
            
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

                        string res = point_intersection(bmp, A, B, C, D);
                        //Console.WriteLine("A:" + A + " B:" + B + " C:" + C + " D:" + D + ";" + res);
                        sb.Add(combs[i] + ";" + combs[j] + ";" + res);
                        //Console.WriteLine(combs[i] + ";" + combs[j] + ";" + res);                       
                    }
                }
            }

            using (StreamWriter sw = File.CreateText(text_file_debug))
            {
                for(int i = 0; i < sb.Count; i ++)
                {
                    sw.WriteLine(sb[i]);
                }
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
                Point qrcode_l = new Point(x_qrcode + Convert.ToInt32(size_qrcode/2) + 5 , y_qrcode - size_qrcode * 2 - size_qrcode - 50);
                Point qrcode_r = new Point(x_qrcode + size_qrcode * 3 + Convert.ToInt32(size_qrcode / 2), y_qrcode - size_qrcode * 2 - size_qrcode - 50);
                Point qrcode_b = new Point(x_qrcode + Convert.ToInt32(size_qrcode / 2), y_qrcode - size_qrcode + Convert.ToInt32(size_qrcode / 2));
                qrcode_points.Add("qrcode" + (i + 1) + "_l", qrcode_l);
                qrcode_points.Add("qrcode" + (i + 1) + "_r", qrcode_r);
                qrcode_points.Add("qrcode" + (i + 1) + "_b", qrcode_b);
            }

            return qrcode_points;
        }



        private string point_intersection(Bitmap bmp, Point A, Point B, Point C, Point D)
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
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                

                if (x < 0)
                    return "0";
                else if (y < 0)
                    return "0";

                if (x <= bmp.Width && y <= bmp.Height)
                {
                    if (x != C.X && y != C.Y && x != D.X && y != D.Y)
                    {
                        //Point inter = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                        return Convert.ToInt32(x) + "," + Convert.ToInt32(y);

                        //bmp.SetPixel(Convert.ToInt32(x), Convert.ToInt32(y), Color.Blue);
                        //Console.WriteLine("Point inter " + inter);
                    }
                    else
                        return "0";
      
                }        
                else return "0";
            }
            return "0";

        }
    }
}
