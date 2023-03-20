using IronBarCode;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing;

namespace WatermarkApp
{
    /// <summary>
    ///  Classe destinada a colocar a marca de agua no ficheiro
    /// </summary>
    public class Watermark
    {
        private string filename;
        private int id_doc;
        private int resizedBarcode = 20;
        private int dpiBarcode = 300;
        private Commom commom;

        public Watermark(string pdf_name, int id)
        {
            filename = pdf_name;
            id_doc = id;
            commom = new Commom();
        }

        public void Generate_barcode(int id_barcode)
        {
            string data_barcode = id_doc.ToString() + ";" + id_barcode.ToString();
            GeneratedBarcode MyBarCode = BarcodeWriter.CreateBarcode(data_barcode, BarcodeWriterEncoding.Code128);
            MyBarCode.ResizeTo(MyBarCode.Width, resizedBarcode); // pixels
            MyBarCode.SetMargins(0, 0, 0, 0);
            MyBarCode.ChangeBarCodeColor(Color.Black);
            MyBarCode.SaveAsPng(filename + commom.extension_barcode);
        }

        
        public void Add_watermark_pdf(string date_time)
        {
            using (Stream inputPdfStream = new FileStream(filename + ".pdf", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream barcodeStream = new FileStream(filename + commom.extension_barcode, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(filename + "_" + commom.extension_watermark + "_" + date_time + ".pdf", FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true; // aceder a documentos confidenciais
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetUnderContent(1);
                int width = (int)reader.GetPageSize(1).Width;
                int height = (int)reader.GetPageSize(1).Height;
                int middle_w = width / 3;

                iTextSharp.text.Image barcode_img = iTextSharp.text.Image.GetInstance(barcodeStream);
                barcode_img.SetAbsolutePosition(middle_w, commom.height_barcode);
                barcode_img.SetDpi(dpiBarcode, dpiBarcode);
                pdfContentByte.AddImage(barcode_img);

                stamper.Close();
                stamper.Dispose();
                reader.Dispose();
                inputPdfStream.Dispose();
            }
        }
    }
}
