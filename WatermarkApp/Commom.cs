using IronBarCode;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace WatermarkApp
{
    /// <summary>
    /// Commom functions between classes
    /// </summary>
    /// 
    public class Commom
    {
        public int width;
        public int height;
        /// <summary>
        /// Here are the files and the jar file
        /// </summary>
        public string files;
        public string files_dir = @"Ficheiros\";
        public string extension_watermark = "watermark";
        public string extension_integrity = "integrity";
        public string extension_barcode = "_barcode.png";

        public int number_points = 9;
        public int height_barcode = 15; // o 0 começa em baixo

        private int x;
        private int y;
        private Point positionBarcode = new Point(210, 800);

        public Commom()
        {
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            files = partialPath + files_dir;
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
            var dd = System.IO.File.ReadAllBytes(file_name_png);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            string[] filename = file_name_png.Split(new[] { ".pdf" }, StringSplitOptions.None);
            if(!System.IO.File.Exists(filename[0] + ".png"))
                System.IO.File.WriteAllBytes(filename[0]+ ".png", pngByte);
            return filename[0] + ".png";
        }

        public string Read_barcode(string file_name)
        {
            BarcodeResult BarCodeResult = BarcodeReader.QuicklyReadOneBarcode(file_name, BarcodeEncoding.Code128, true);
            if (BarCodeResult != null)
            {
                return BarCodeResult.Value;
            }
            return "insucesso";
        }

        public string Return_PositionBarcode(string file_name)
        {
          
            string img = Convert_pdf_png(file_name);
            GetDimensionsDocument(file_name);
            BarcodeResult BarCodeResult = BarcodeReader.QuicklyReadOneBarcode(img, BarcodeEncoding.Code128, true);
            if (BarCodeResult != null)
            {
                Bitmap bmp = new Bitmap(img);
                x = (int)BarCodeResult.X1 * width / bmp.Width;
                y = (int)BarCodeResult.Y1 * height / bmp.Height;
                int x2 = (int)BarCodeResult.X2 * width / bmp.Width;
                int y2 = (int)BarCodeResult.Y2 * height / bmp.Height;
                bmp.Dispose();

                if(!file_name.Contains("scan"))
                {
                    x = positionBarcode.X;
                    y = positionBarcode.Y;
                }

                return $"{x}:{y}:{x2}:{y2}";
            }
           
            return "insucesso";
        }


        public void RetificateAnalise(int id_doc, SQL_connection sql, string file_name, int difference_x, int difference_y, double prop_x, double prop_y, double angle)
        {   
            List<string> returnlist = sql.Get_Values_Analise_Forense(id_doc);
            AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name);

            string img = auxFunc.DrawImage(returnlist, file_name, difference_x, difference_y, prop_x, prop_y, angle);
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
