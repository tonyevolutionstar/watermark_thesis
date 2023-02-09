using IronBarCode;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using System.IO;

namespace WatermarkApp
{
    /// <summary>
    ///  Classe destinada a colocar a marca de agua no ficheiro
    /// </summary>
    public class Watermark
    {
        private string filename = "";
        private int id_doc;

        public Watermark(string pdf_name, int id)
        {
            filename = pdf_name;
            id_doc = id;
        }


        public void Generate_barcode(int id_barcode)
        {  
            string data_barcode = id_doc.ToString() + ";" + id_barcode;
            GeneratedBarcode MyBarCode = BarcodeWriter.CreateBarcode(data_barcode, BarcodeWriterEncoding.Code128);
            MyBarCode.ResizeTo(MyBarCode.Width,5); // pixels
            MyBarCode.ChangeBarCodeColor(Color.Black);
            MyBarCode.SaveAsPng(filename + "_barcode.png");  
        }

        
        public void Add_watermark_pdf(string date_time)
        {
            using (Stream inputPdfStream = new FileStream(filename + ".pdf", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream barcodeStream = new FileStream(filename + "_barcode.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(filename + "_watermark_" + date_time + ".pdf", FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true;
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetUnderContent(1);
                int width = (int)reader.GetPageSize(1).Width;
                int height = (int)reader.GetPageSize(1).Height;

                float m = width / 3;
                int middle_w = (int)Math.Round(m);
                Commom commom = new Commom();
                string img_file = commom.Convert_pdf_png(filename + ".pdf");

                iTextSharp.text.Image barcode_img = iTextSharp.text.Image.GetInstance(barcodeStream);
                barcode_img.SetAbsolutePosition(middle_w, 15);
                barcode_img.SetDpi(100, 100);
                pdfContentByte.AddImage(barcode_img);

                stamper.Close();
                stamper.Dispose();
                reader.Dispose();
                inputPdfStream.Dispose();
            }
        }
    }
}
