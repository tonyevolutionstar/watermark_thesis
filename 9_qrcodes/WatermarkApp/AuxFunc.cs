using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WatermarkApp
{
    public class AuxFunc
    {
        private string file_name;
        private int size_qrcode;
        private int h;
        private int w;

        public AuxFunc(string filename, int size_qrcode)
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

            Dictionary<string, Point> qrcode_points = FillDictionary(position, bmp);
            
            string qrcode_comb;
            List<string> combs = new List<string>();

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
                int x_qrcode = int.Parse(qrcode[0]) * bmp.Width / w;
                int y_qrcode = int.Parse(qrcode[1]) * bmp.Height / h;
                Point qrcode_l = new Point(x_qrcode + this.size_qrcode, y_qrcode - this.size_qrcode * 3);
                Point qrcode_r = new Point(x_qrcode + this.size_qrcode * 3, y_qrcode - this.size_qrcode * 3);
                Point qrcode_b = new Point(x_qrcode + this.size_qrcode, y_qrcode - this.size_qrcode);
                qrcode_points.Add("qrcode" + (i + 1) + "_l", qrcode_l);
                qrcode_points.Add("qrcode" + (i + 1) + "_r", qrcode_r);
                qrcode_points.Add("qrcode" + (i + 1) + "_b", qrcode_b);
            }

            return qrcode_points;
        }


        private void ddaline(Point p1, Point p2, Graphics g, Bitmap bmp)
        {
            Pen redPen = new Pen(Color.Red, 3);

            double xinc, yinc, x, x1, y, y1, steps;
            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;
            if (deltaX > deltaY) steps = deltaX;
            else steps = deltaY;

            xinc = deltaX / steps;
            yinc = deltaY / steps;
            x = x1 = p1.X;
            y = y1 = p1.Y;

            for (int k = 1; k <= steps; k++)
            {
                x = x + xinc;
                x = Math.Round(x, 0);
                y = y + yinc;
                y = Math.Round(y, 0);

                Console.WriteLine("line coord: " + x + "," + y);
                bmp.SetPixel((int)(x * w), (int)(y * h), Color.DarkBlue);
            }
        }
    }
}
