using ZXing;
using ZXing.Common; 
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WatermarkApp
{
    /// <summary>
    /// Funções comuns entre classes
    /// </summary>
    /// 
    public class Commom
    {
        public int width; //comprimento do ficheiro
        public int height; //altura do ficheiro
        public string files;
        public string files_dir = @"Ficheiros\";
        public string extension_watermark = "watermark";
        public string extension_integrity = "integrity";
        public string extension_barcode = "_barcode.png";
        public int number_points = 9;
        public int height_barcode = 15; // o 0 começa em baixo
        public string errorReadBarcode = "insucesso";
        private string jar_file = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Ficheiros\thesis_watermark.jar";
        public int width_bmp;
        public int height_bmp;
        public int x_barcode_pos = 200;
        public int y_barcode_pos = 812;
        public int x2_barcode_pos = 446;
        public int y2_barcode_pos = 827;
        public int x_39 = 232;
        public int y_39 = 5;
        public int x2_39 = 413;
        public int y2_39 = 20;


        private TrackerServices trackerServices = new TrackerServices();

        public Commom()
        {
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            files = partialPath + files_dir;
        }

        /// <summary>
        /// Executa o ficheiro jar que vai gerar um ficheiro txt com as posições dos 
        /// caracteres que estão no pdf
        /// </summary>
        public void Get_PositionChar(string file_name)
        {
            System.Diagnostics.Process process_file = new System.Diagnostics.Process();
            process_file.StartInfo.UseShellExecute = false;
            process_file.StartInfo.RedirectStandardOutput = true;
            process_file.StartInfo.FileName = "java";
            process_file.StartInfo.Arguments = "-jar " + '"' + jar_file + '"' + " " + '"' + file_name + '"';
            process_file.Start();
            process_file.WaitForExit();
        }


        /// <summary>
        /// Lê o ficheiro gerado das posições dos caracteres no documento e colocar os valores numa variavel 
        /// </summary>
        /// <returns>Linhas do documento</returns>
        public string[] Read_positionChar_file(string file_name)
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string posFile = s_doc[0] + "_pos.txt";
            string[] lines = System.IO.File.ReadAllLines(posFile, System.Text.Encoding.UTF7);
            return lines;
        }


        public string Get_file_name_using_split(string file_name)
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            return s_doc[0];
        }


        public string Get_file_name_without_directory(string file_name)
        {
            string[] show_doc = file_name.Split(new[] {files_dir }, StringSplitOptions.None);
            string[] file = show_doc[1].Split(new[] { ".pdf" }, StringSplitOptions.None);
            return file[0];
        }

        public string Convert_pdf_png(string file_name_png)
        {
            trackerServices.WriteFile($"converção do ficheiro {file_name_png} concluída");
            var dd = System.IO.File.ReadAllBytes(file_name_png);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            string[] filename = file_name_png.Split(new[] { ".pdf" }, StringSplitOptions.None);
            if(!System.IO.File.Exists(filename[0] + ".png"))
                System.IO.File.WriteAllBytes(filename[0]+ ".png", pngByte);
            return filename[0] + ".png";
        }

        public string Read_barcode(string file_name)
        {
            trackerServices.WriteFile($"lendo o código de barras do ficheiro {file_name}");
            string img_file = Convert_pdf_png(file_name);
            Bitmap bmp = new Bitmap(img_file);
            var reader = new BarcodeReader
            {
                Options = new DecodingOptions
                {
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128 },
                    TryHarder = true
                }
            };

            // Decode the barcode from the image
            var result = reader.Decode(bmp);

            if (result != null)
            {
                bmp.Dispose();
                return result.Text;
            }
            else
                trackerServices.WriteFile("erro na leitura do código de barras");

            return errorReadBarcode;
        }


        public string Return_PositionBarcode(string file_name)
        {
            GetDimensionsDocument(file_name);
            trackerServices.WriteFile("lendo as posições do código de barras");
            string img_file = Convert_pdf_png(file_name);
            Bitmap bmp = new Bitmap(img_file);
            var reader128 = new BarcodeReader
            {
                Options = new DecodingOptions
                {
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128 },
                    TryHarder = true
                }
            };
            var reader39 = new BarcodeReader
            {
                Options = new DecodingOptions
                {
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_39 },
                    TryHarder = true
                }
            };
            var barcodeResult128 = reader128.Decode(bmp);
            var result = barcodeResult128?.ResultPoints;
            var barcodeResult39 = reader39.Decode(bmp);
            var result2 = barcodeResult39?.ResultPoints;
            List<Point> list128 = new List<Point>();
            List<Point> list39 = new List<Point>();
            Point p1_barcode129 = new Point();
            Point p2_barcode129 = new Point();
            Point p1_barcode39 = new Point();
            Point p2_barcode39 = new Point();

            if (result != null && result2 != null)
            {
                foreach (var point in result)
                {
                    int x = (int)point.X * width / bmp.Width;
                    int y = (int)point.Y * height / bmp.Height;
                    list128.Add(new Point(x, y));
                }
                foreach (var point2 in result2)
                {
                    int x = (int)point2.X * width / bmp.Width;
                    int y = (int)point2.Y * height / bmp.Height;
                    list39.Add(new Point(x, y));
                }

                for (int i = 0; i < list128.Count; i++)
                {
                    p1_barcode129 = list128[0];
                    p2_barcode129 = list128[1];
                }

                for (int i = 0; i < list39.Count; i++)
                {
                    p1_barcode39 = list39[0];
                    p2_barcode39 = list39[1];
                }
            }
            else
            {
                trackerServices.WriteFile("erro ao obter as posições do código de barras");
                return "";
            }

            x_barcode_pos = p1_barcode129.X * width / bmp.Width + 95;
            y_barcode_pos = p1_barcode129.Y * height / bmp.Height + 26;
            x2_barcode_pos = p2_barcode129.X * width / bmp.Width - 62;
            y2_barcode_pos = (p2_barcode129.Y + 15) * height / bmp.Height + 3;

            x_39 = p1_barcode39.X * width / bmp.Width + 100;
            y_39 = p1_barcode39.Y * height / bmp.Height + 25;
            x2_39 = p2_barcode39.X * width / bmp.Width - 70;
            y2_39 = (p2_barcode39.Y + 15) * height / bmp.Height + 3;

            if (file_name.Contains("scan"))
            {
                x_barcode_pos = p1_barcode129.X * width / bmp.Width + 95;
                y_barcode_pos = p1_barcode129.Y * height / bmp.Height + 26;
                x2_barcode_pos = p2_barcode129.X * width / bmp.Width - 62;
                y2_barcode_pos = (p2_barcode129.Y + 15) * height / bmp.Height + 3;

                x_39 = p1_barcode39.X * width / bmp.Width + 100;
                y_39 = p1_barcode39.Y * height / bmp.Height + 25;
                x2_39 = p2_barcode39.X * width / bmp.Width - 70;
                y2_39 = (p2_barcode39.Y + 15) * height / bmp.Height + 3;

                if (y2_39 < 0)
                {
                    y2_39 += 19;
                }

                //change
                //fixing the values of scan image 
                if (y_barcode_pos >= 1000)
                    y_barcode_pos = Math.Abs(height - y_barcode_pos + 15);
                if (x_barcode_pos >= 220)
                    x_barcode_pos = Math.Abs(width - x_barcode_pos - 63);
                if (x2_barcode_pos >= 500)
                    x2_barcode_pos = Math.Abs(width - x2_barcode_pos);
                if (y2_barcode_pos >= 1000)
                    y2_barcode_pos = Math.Abs(height - y2_barcode_pos + 90);
                if (x2_barcode_pos <= 100)
                    x2_barcode_pos += width / 2 + 41;
            }
            bmp.Dispose();

            return $"{x_barcode_pos}:{y_barcode_pos}:{x2_barcode_pos}:{y2_barcode_pos}:{x_39}:{y_39}:{x2_39}:{y2_39}";
        }


        public void RetificateAnalise(int id_doc, SQL_connection sql, string file_name, int difference_x, int difference_y, double prop_x, double prop_y, int diff_width_doc, int diff_height_doc, int diff_width_bmp, int diff_height_bmp)
        {   
            List<string> returnlist = sql.Get_Values_Analise_Forense(id_doc);
            AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name);

            string img = auxFunc.DrawImage(returnlist, file_name, difference_x, difference_y, prop_x, prop_y, diff_width_doc, diff_height_doc, diff_width_bmp, diff_height_bmp);
            string[] filename = img.Split(new[] { ".png" }, StringSplitOptions.None);

            string output = filename[0] + ".pdf";
            string[] s_doc = file_name.Split(new[] { "_integrity" }, StringSplitOptions.None);

            using (FileStream sourceStream = new FileStream(s_doc[0], FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (FileStream outputStream = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
                    PdfStamper stamper = new PdfStamper(reader, outputStream);
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(img);
                    image.SetAbsolutePosition(0, 0);
                    iTextSharp.text.Rectangle pageSize = reader.GetPageSizeWithRotation(1);
                    image.ScaleToFit(pageSize.Width, pageSize.Height);
                    PdfContentByte content = stamper.GetOverContent(1);
                    content.AddImage(image);
                    stamper.Close();
                    reader.Close();
                }
            }

            AnaliseForenseForm form = new AnaliseForenseForm(filename[0] + ".pdf");
            form.Show();
        }

        public void GetDimensionsImage(string file_name)
        {
            if (!file_name.Contains(".pdf"))
                file_name += ".pdf";
            string img_file = Convert_pdf_png(file_name);
            Bitmap bmp = new Bitmap(img_file);
            width_bmp = bmp.Width;
            height_bmp = bmp.Height;
            bmp.Dispose();
        }

        public void GetDimensionsDocument(string file_name)
        {
            if (!file_name.Contains(".pdf"))
                file_name += ".pdf";
            using (Stream inputPdfStream = new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new iTextSharp.text.pdf.PdfReader(inputPdfStream);
                iTextSharp.text.pdf.PdfReader.unethicalreading = true;
                width = (int)reader.GetPageSize(1).Width;
                height = (int)reader.GetPageSize(1).Height;
                reader.Close();
                reader.Dispose();
            }
        }
    }
}
