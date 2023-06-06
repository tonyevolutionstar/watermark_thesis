using ZXing;
using ZXing.Common; 
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Bytescout.BarCodeReader;

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

        #region PositionBarcode
        public int x_barcode_pos;
        public int y_barcode_pos;
        public int x2_barcode_pos;
        public int y2_barcode_pos;
        public int x_39;
        public int y_39;
        public int x2_39;
        public int y2_39;
        #endregion

        private TrackerServices trackerServices = new TrackerServices();

        public Commom()
        {
            files = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + files_dir;
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

        private string Get_file_img_name(string img_file)
        {
            string[] filename = img_file.Split(new[] { ".png" }, StringSplitOptions.None);
            return filename[0] + ".pdf";
        }

        public string Convert_pdf_png(string file_name_png)
        {
            trackerServices.WriteFile($"converção do ficheiro {file_name_png} concluída");
            var dd = File.ReadAllBytes(file_name_png);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            string[] filename = file_name_png.Split(new[] { ".pdf" }, StringSplitOptions.None);
            if(!File.Exists(filename[0] + ".png"))
                File.WriteAllBytes(filename[0]+ ".png", pngByte);
            return filename[0] + ".png";
        }

        public string Read_barcode(string img_file)
        {
            string file_name = Get_file_img_name(img_file);
            trackerServices.WriteFile($"lendo o código de barras do ficheiro {file_name}");
            Bitmap bmp = new Bitmap(img_file);

            var reader = new BarcodeReader
            {
                Options = new DecodingOptions
                {
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128 },
                    TryHarder = true
                }
            };

            Result result = reader.Decode(bmp);

            if (result != null)
            {
                return result.Text;
            }
            else
                trackerServices.WriteFile("erro na leitura do código de barras");

            bmp.Dispose();
            return errorReadBarcode;
        }


        public string Return_PositionBarcode(string img_file)
        {
            string file_name = Get_file_img_name(img_file);
            GetDimensionsDocument(file_name);
            trackerServices.WriteFile($"lendo as posições do código de barras do ficheiro {file_name}");
            Bitmap bmp = new Bitmap(img_file);
            var reader128 = new BarcodeReader
            {
                Options = new DecodingOptions
                {
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128 },
                    TryHarder = true
                }
            };

            // devido a falhas de leitura do código de barras do zxing.net em certos documentos com escala variavel 
            // usou-se o Bytescout.BarCodeReader para ler
            var reader39 = new Bytescout.BarCodeReader.Reader();
            reader39.BarcodeTypesToFind.Code39 = true;
            reader39.MaxNumberOfBarcodesPerPage = 1;

            var result2 = reader39.ReadFrom(bmp);

            var barcodeResult128 = reader128.Decode(bmp);
            var result = barcodeResult128?.ResultPoints;
         
            List<Point> list128 = new List<Point>();
            List<Point> list39 = new List<Point>();
            Point p1_barcode128 = new Point();
            Point p2_barcode128 = new Point();
            Point p1_barcode39 = new Point();

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
                    int x = point2.Rect.X * width / bmp.Width;
                    int y = point2.Rect.Y * height / bmp.Height;
                    list39.Add(new Point(x, y));
                }

                for (int i = 0; i < list128.Count; i++)
                {
                    p1_barcode128 = list128[0];
                    p2_barcode128 = list128[1];
                }

                for (int i = 0; i < list39.Count; i++)
                {
                    p1_barcode39 = list39[0];
                }
            }
            else
            {
                trackerServices.WriteFile("erro ao obter as posições do código de barras");
                return "";
            }

            x_barcode_pos = p1_barcode128.X - 10;
            y_barcode_pos = p1_barcode128.Y - 2;
            x2_barcode_pos = p2_barcode128.X + 16;
            y2_barcode_pos = p2_barcode128.Y - 2 + 15;

            x_39 = p1_barcode39.X + 1;
            y_39 = p1_barcode39.Y;
            x2_39 = p1_barcode39.X + 195;
            y2_39 = y_39 + 15;

            if (file_name.Contains("scan"))
            {
                x_barcode_pos = p1_barcode128.X - 10;
                y_barcode_pos = p1_barcode128.Y;
                x2_barcode_pos = p2_barcode128.X + 16;
                y2_barcode_pos = p2_barcode128.Y + 15;

                x_39 = p1_barcode39.X + 1;
                y_39 = p1_barcode39.Y+1;
                x2_39 = p1_barcode39.X + 195;
                y2_39 = y_39 + 15;
            }
            bmp.Dispose();

            return $"{x_barcode_pos}:{y_barcode_pos}:{x2_barcode_pos}:{y2_barcode_pos}:{x_39}:{y_39}:{x2_39}:{y2_39}";
        }


        public void RetificateAnalise(int id_doc, SQL_connection sql, string file_name, int difference_x, int difference_y, double prop_x, double prop_y, int diff_width_doc, int diff_height_doc, int diff_width_bmp, int diff_height_bmp, double scale_doc)
        {   
            List<string> returnlist = sql.Get_Values_Analise_Forense(id_doc);
            AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name);

            string img = auxFunc.DrawImage(returnlist, file_name, difference_x, difference_y, prop_x, prop_y, diff_width_doc, diff_height_doc, diff_width_bmp, diff_height_bmp, scale_doc);
            string[] filename = img.Split(new[] { ".png" }, StringSplitOptions.None);

            string output = filename[0] + ".pdf";
            string[] s_doc = file_name.Split(new[] { "_integrity" }, StringSplitOptions.None);

            using (FileStream sourceStream = new FileStream(s_doc[0], FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (FileStream outputStream = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfReader reader = new PdfReader(sourceStream);
                    PdfReader.unethicalreading = true;
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

            VerificacaoIntegridade form = new VerificacaoIntegridade(filename[0] + ".pdf");
            form.Show();
        }

        public void GetDimensionsImage(string img_file)
        {
            Bitmap bmp = new Bitmap(img_file);
            width_bmp = bmp.Width;
            height_bmp = bmp.Height;
            bmp.Dispose();
        }

        public void GetDimensionsDocument(string file_name)
        {
            using (Stream inputPdfStream = new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true;
                width = (int)reader.GetPageSize(1).Width;
                height = (int)reader.GetPageSize(1).Height;
                reader.Close();
                reader.Dispose();
            }
        }
    }
}
