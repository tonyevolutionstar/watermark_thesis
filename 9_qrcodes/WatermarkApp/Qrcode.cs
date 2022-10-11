﻿using IronBarCode;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace WatermarkApp
{
    /// <summary>
    ///  Classe destinada a gerar o qrcode e coloca-lo no documento numa posição aleatória
    /// </summary>
    public class QRcode
    {
        private string filename = "";
        private int id_doc;
        private int sizeArc = 20;
        private int w = 0;
        private int h = 0;

        /// <summary>
        /// posição que vai ser inserido o qrcode no ficheiro
        /// </summary>
     
        private int version = 2;
        private int size_qrcode; //Width == height para ser quadrado
        private int range; 

        /// <summary>
        /// lista de posiçoes do qrcode
        /// </summary>
        public List<string> pos = new List<string>();

        /// <summary>
        /// O construtor do qrcode vai receber um ficheiro e vai colocar esse ficheiro em classe, para ser possivel aceder
        /// </summary>
        /// <param name="pdf_name">nome do ficheiro</param>
        /// <param name="size">tamanho do qrcode</param>
        /// <param name="id">id identificador do documento</param>
        /// <param name="range">posicao a ver o character</param>
        /// <returns>nome do ficheiro</returns>
        public QRcode(string pdf_name, int size, int id, int range)
        {
            filename = pdf_name;
            size_qrcode = size;
            pos = new List<string>();
            id_doc = id;
            this.range = range;
        }


        /// <summary>
        /// Guarda uma imagem do qrcode gerado com base nas caracteristicas do ficheiro
        /// Estrutura do qrcode é version;id_doc;info_char;range
        /// <param name="info_char">caracter a x direita do qrcode</param> 
        /// <param name="i">numero identificador do qrcode, max até 9</param> 
        /// <returns>imagem qrcode</returns>
        /// </summary>

        public void Generate_qrcode(char info_char, int i)
        {
            if(version == 2)
            {
                string data_qrcode = version + ";" + id_doc + ";" + info_char + ";" + range;

                string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string logoPath = System.IO.Path.Combine(partialPath, @"number_qrcode\" + i.ToString() + ".png");
                Bitmap bmp = new Bitmap(logoPath);

                var MyBarCode = QRCodeWriter.CreateQrCodeWithLogoImage(data_qrcode, bmp, size_qrcode);
  
                MyBarCode.ResizeTo(size_qrcode, size_qrcode); // pixels
                MyBarCode.ChangeBarCodeColor(Color.LightGray);
                
                MyBarCode.SaveAsPng(filename + "_qrcode_" + i + ".png");
                bmp.Dispose();
            }
        }   

        /// <summary>
        /// Cria um código de barras
        /// </summary>
        /// <param name="id_barcode"></param>
        public void Generate_barcode(int id_barcode)
        {  
            string data_qrcode = id_doc.ToString() + ";" + id_barcode;
            GeneratedBarcode MyBarCode = BarcodeWriter.CreateBarcode(data_qrcode, BarcodeWriterEncoding.Code128);
            MyBarCode.ResizeTo(MyBarCode.Width,5); // pixels
            MyBarCode.ChangeBarCodeColor(Color.Black);
            MyBarCode.SaveAsPng(filename + "_barcode.png");  
        }

        /// <summary>
        /// Vai ler o ficheiro original e a imagem do qrcode, colocando o qrcode numa posição aleatória num ficheiro novo
        /// </summary>
        /// <param name="positions">posicoes qrcode</param>
        /// <returns>nome do ficheiro + "_qrcode.pdf"</returns>
        public void Add_barcodes_pdf(string positions)
        {
            using (Stream inputPdfStream = new FileStream(filename + ".pdf", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_1 = new FileStream(filename + "_qrcode_1.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_2 = new FileStream(filename + "_qrcode_2.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_3 = new FileStream(filename + "_qrcode_3.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_4 = new FileStream(filename + "_qrcode_4.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_5 = new FileStream(filename + "_qrcode_5.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_6 = new FileStream(filename + "_qrcode_6.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_7 = new FileStream(filename + "_qrcode_7.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_8 = new FileStream(filename + "_qrcode_8.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream qrcodeStream_9 = new FileStream(filename + "_qrcode_9.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream barcodeStream = new FileStream(filename + "_barcode.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(filename + "_qrcode.pdf", FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true;
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetUnderContent(1);
                int width = (int)reader.GetPageSize(1).Width;
                int height = (int)reader.GetPageSize(1).Height;
                w = width;
                h = height;

                float m = width / 3;
                int middle_w = (int)Math.Round(m);

                string[] positions_qrcode = positions.Split('|');

                iTextSharp.text.Image qr_code_img_1 = iTextSharp.text.Image.GetInstance(qrcodeStream_1);
                string[] qrcode1 = positions_qrcode[0].Split(',');
                qr_code_img_1.SetAbsolutePosition(Int16.Parse(qrcode1[0]), Int16.Parse(qrcode1[1]));
                pdfContentByte.AddImage(qr_code_img_1);

                iTextSharp.text.Image qr_code_img_2 = iTextSharp.text.Image.GetInstance(qrcodeStream_2);
                string[] qrcode2 = positions_qrcode[1].Split(',');
                qr_code_img_2.SetAbsolutePosition(Int16.Parse(qrcode2[0]), Int16.Parse(qrcode2[1]));
                pdfContentByte.AddImage(qr_code_img_2);

                iTextSharp.text.Image qr_code_img_3 = iTextSharp.text.Image.GetInstance(qrcodeStream_3);
                string[] qrcode3 = positions_qrcode[2].Split(',');
                qr_code_img_3.SetAbsolutePosition(Int16.Parse(qrcode3[0]), Int16.Parse(qrcode3[1]));
                pdfContentByte.AddImage(qr_code_img_3);

                iTextSharp.text.Image qr_code_img_4 = iTextSharp.text.Image.GetInstance(qrcodeStream_4);
                string[] qrcode4 = positions_qrcode[3].Split(',');
                qr_code_img_4.SetAbsolutePosition(Int16.Parse(qrcode4[0]), Int16.Parse(qrcode4[1]));
                pdfContentByte.AddImage(qr_code_img_4);

                iTextSharp.text.Image qr_code_img_5 = iTextSharp.text.Image.GetInstance(qrcodeStream_5);
                string[] qrcode5 = positions_qrcode[4].Split(',');
                qr_code_img_5.SetAbsolutePosition(Int16.Parse(qrcode5[0]), Int16.Parse(qrcode5[1]));
                pdfContentByte.AddImage(qr_code_img_5);

                iTextSharp.text.Image qr_code_img_6 = iTextSharp.text.Image.GetInstance(qrcodeStream_6);
                string[] qrcode6 = positions_qrcode[5].Split(',');
                qr_code_img_6.SetAbsolutePosition(Int16.Parse(qrcode6[0]), Int16.Parse(qrcode6[1]));
                pdfContentByte.AddImage(qr_code_img_6);

                iTextSharp.text.Image qr_code_img_7 = iTextSharp.text.Image.GetInstance(qrcodeStream_7);
                string[] qrcode7 = positions_qrcode[6].Split(',');
                qr_code_img_7.SetAbsolutePosition(Int16.Parse(qrcode7[0]), Int16.Parse(qrcode7[1]));
                pdfContentByte.AddImage(qr_code_img_7);

                iTextSharp.text.Image qr_code_img_8 = iTextSharp.text.Image.GetInstance(qrcodeStream_8);
                string[] qrcode8 = positions_qrcode[7].Split(',');
                qr_code_img_8.SetAbsolutePosition(Int16.Parse(qrcode8[0]), Int16.Parse(qrcode8[1]));
                pdfContentByte.AddImage(qr_code_img_8);

                iTextSharp.text.Image qr_code_img_9 = iTextSharp.text.Image.GetInstance(qrcodeStream_9);
                string[] qrcode9 = positions_qrcode[8].Split(',');
                qr_code_img_9.SetAbsolutePosition(Int16.Parse(qrcode9[0]), Int16.Parse(qrcode9[1]));
                pdfContentByte.AddImage(qr_code_img_9);

                iTextSharp.text.Image barcode_img = iTextSharp.text.Image.GetInstance(barcodeStream);
                barcode_img.SetAbsolutePosition(middle_w, 5);
                pdfContentByte.AddImage(barcode_img);

                stamper.Close();
                stamper.Dispose();
                reader.Dispose();
              
                inputPdfStream.Dispose();
            }
        }


        /// <summary>
        /// Converte o ficheiro pdf para png
        /// </summary>
        /// <param name="f">Nome do ficheiro</param>
        /// <returns>Nome do ficheiro + ".png"</returns>
        private string Convert_pdf_png(string f)
        {
            var dd = System.IO.File.ReadAllBytes(f+".pdf");
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1); 
            string[] filename = f.Split(new[] { ".pdf" }, StringSplitOptions.None);
            System.IO.File.WriteAllBytes(filename[0] + ".png", pngByte);
            return filename[0] + ".png";
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
               // g.DrawArc(yellowPen, iPoint.X - sizeArc, iPoint.Y - sizeArc, 2 * sizeArc, 2 * sizeArc, 0, 360);
                //g.DrawArc(redPen, (float)x - sizeArc, (float)y - sizeArc, 2 * sizeArc, 2 * sizeArc, 0, 360);
                bmp.SetPixel((int)(x*w), (int)(y*h), Color.DarkBlue);
            }
        }

        private Dictionary<string, Point> FillDictionary(string position, Bitmap bmp)
        {
            Dictionary<string, Point> qrcode_points = new Dictionary<string, Point>();
            for(int i = 0; i < 9; i ++)
            {
                string[] pos_qrcodes = position.Split('|');
                string[] qrcode = pos_qrcodes[i].Split(',');
                int x_qrcode = int.Parse(qrcode[0]) * bmp.Width / w;
                int y_qrcode = int.Parse(qrcode[1]) * bmp.Height / h;
                Point qrcode_l = new Point(x_qrcode + size_qrcode, y_qrcode - size_qrcode * 3);
                Point qrcode_r = new Point(x_qrcode + size_qrcode * 3, y_qrcode - size_qrcode * 3);
                Point qrcode_b = new Point(x_qrcode + size_qrcode, y_qrcode - size_qrcode);
                qrcode_points.Add("qrcode" + (i + 1) + "_l", qrcode_l);
                qrcode_points.Add("qrcode" + (i + 1) + "_r", qrcode_r);
                qrcode_points.Add("qrcode" + (i + 1) + "_b", qrcode_b);
            }

            return qrcode_points;
        }


        /// <summary>
        /// desenha linhas
        /// </summary>
        /// <param name="position"></param>
        /// <param name="file_name"></param>
        public void DrawLines(string position, string file_name)
        {
            string f = Convert_pdf_png(file_name);
            Bitmap bmp = new Bitmap(f);
            Graphics g = Graphics.FromImage(bmp);

            Pen redPen = new Pen(Color.Red, 3);
            Pen bluePen = new Pen(Color.Blue, 3);
            Pen greenPen = new Pen(Color.Green, 3);
            Pen yellowPen = new Pen(Color.Yellow, 3);
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
                        //Console.WriteLine(qrcode_comb);
                        if(!combs.Contains(entry2.Key + ":" + entry.Key))
                            combs.Add(qrcode_comb);
                    }
                }
            }

            for(int i = 0; i < combs.Count; i++)
            {
                //Console.WriteLine(combs[i]);
                string[] points = combs[i].Split(':');

                qrcode_points.TryGetValue(points[0], out Point p1);
                qrcode_points.TryGetValue(points[1], out Point p2);
                //Console.WriteLine(p1 + ":" + p2);
                g.DrawLine(greenPen, p1, p2);
                //ddaline(p1, p2, g, bmp);
            }

            string[] filename = f.Split(new[] { ".png" }, StringSplitOptions.None);

            bmp.Save(filename[0] + "line.png");
            bmp.Dispose();
        }
    }
}
