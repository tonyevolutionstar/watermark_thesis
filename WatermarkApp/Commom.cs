using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;

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

        private TrackerServices trackerServices = new TrackerServices();

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
            PdfReader reader = new PdfReader(file_name);
            PdfDictionary page = reader.GetPageN(1);
            PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
            PdfDictionary xObjects = resources.GetAsDict(PdfName.XOBJECT);

            foreach (PdfName name in xObjects.Keys)
            {
                PdfObject obj = xObjects.GetDirectObject(name);

                if (obj.IsStream())
                {
                    PdfDictionary imgObject = (PdfDictionary)PdfReader.GetPdfObject(obj);
                    if (imgObject != null && imgObject.Get(PdfName.SUBTYPE).Equals(PdfName.IMAGE))
                    {
                        PdfImageObject imageObject = new PdfImageObject((PRStream)imgObject);
                        Bitmap image = (Bitmap)imageObject.GetDrawingImage();
                        BarcodeReader barcodeReader = new BarcodeReader();
                        barcodeReader.AutoRotate = true; // Allow rotation of barcode image

                        barcodeReader.Options = new DecodingOptions
                        {
                            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128 }, // Specify the barcode format to read
                            TryHarder = true // check on scan documents
                        };
                        var barcodeRes = barcodeReader.Decode(image);
                        if (barcodeRes != null)
                        {
                            Console.WriteLine("Barcode data: " + barcodeRes.Text);
                            return barcodeRes.Text;
                        }
                    }
                }
            }

            return errorReadBarcode;
        }


        public string Return_PositionBarcode(string file_name)
        {
            GetDimensionsDocument(file_name);
            PdfReader reader = new PdfReader(file_name);
            PdfDictionary page = reader.GetPageN(1);
            PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
            PdfDictionary xObjects = resources.GetAsDict(PdfName.XOBJECT);

            foreach (PdfName name in xObjects.Keys)
            {
                PdfObject obj = xObjects.GetDirectObject(name);

                if (obj.IsStream())
                {
                    PdfDictionary imgObject = (PdfDictionary)PdfReader.GetPdfObject(obj);
                    if (imgObject != null && imgObject.Get(PdfName.SUBTYPE).Equals(PdfName.IMAGE))
                    {
                        PdfImageObject imageObject = new PdfImageObject((PRStream)imgObject);
                        Bitmap image = (Bitmap)imageObject.GetDrawingImage();
                        BarcodeReader barcodeReader = new BarcodeReader();
                        barcodeReader.AutoRotate = true; // Allow rotation of barcode image

                        barcodeReader.Options = new DecodingOptions
                        {
                            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128 }, // Specify the barcode format to read
                            TryHarder = true // check on scan documents
                        };
                        var barcodeRes = barcodeReader.Decode(image);
                        if (barcodeRes != null)
                        {
                            if (file_name.Contains("scan"))
                            {
                                int x1 = (int)barcodeRes.ResultPoints[0].X * width / image.Width + 160;
                                int y1 = height- (int)barcodeRes.ResultPoints[0].Y ;
                                int x2 = (int)barcodeRes.ResultPoints[1].X * width / image.Width - 109;
                                int y2 = (int)barcodeRes.ResultPoints[1].Y * height / image.Height;
                                return $"{x1}:{y1}:{x2}:{y2}";
                            }
                            else
                            {
                                int x1 = (int)barcodeRes.ResultPoints[0].X * width / image.Width + 160;
                                int y1 = height - (int)barcodeRes.ResultPoints[0].Y - 30;
                                int x2 = (int)barcodeRes.ResultPoints[1].X * width / image.Width - 109;
                                int y2 = (int)barcodeRes.ResultPoints[1].Y;
                                return $"{x1}:{y1}:{x2}:{y2}";
                            }
                        }
                    }
                }
            }

            return errorReadBarcode;
        }


        public void RetificateAnalise(int id_doc, SQL_connection sql, string file_name, int difference_x, int difference_y)
        {   
            List<string> returnlist = sql.Get_Values_Analise_Forense(id_doc);
            AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name);

            string img = auxFunc.DrawImage(returnlist, file_name, difference_x, difference_y);
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
