using iTextSharp.text.pdf;
using System.IO;
using ZXing;
using ZXing.Common;

namespace ConsoleNet
{
    public class Watermark
    {
        private readonly string filename;
        private readonly int id_doc;
        private readonly int resizedBarcode = 15;
        private readonly int dpiBarcode = 300;
        private readonly Commom commom;

        public Watermark(string pdf_name, int id)
        {
            filename = pdf_name;
            id_doc = id;
            commom = new Commom();
        }

        public void Generate_barcode(int id_barcode)
        {
            string data_barcode = id_doc.ToString() + ";" + id_barcode.ToString();
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = resizedBarcode,
                    Width = 250,
                    Margin = 0
                }
            };

            var barcodeBitmap = writer.Write(data_barcode);
            barcodeBitmap.Save(filename + commom.extension_barcode);
        }
        public void Generate_barcode_39(int id_barcode)
        {
            string data_barcode = id_doc.ToString() + ";" + id_barcode.ToString();
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_39,
                Options = new EncodingOptions
                {
                    Height = resizedBarcode,
                    Width = 181,
                    Margin = 0
                }
            };

            var barcodeBitmap = writer.Write(data_barcode);
            barcodeBitmap.Save(filename + "_code39.png");
        }


        public void Add_watermark_pdf(string date_time)
        {
            using (Stream inputPdfStream = new FileStream(filename + ".pdf", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream barcodeStream = new FileStream(filename + commom.extension_barcode, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream barcodeStream2 = new FileStream(filename + "_code39.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(filename + "_" + commom.extension_watermark + "_" + date_time + ".pdf", FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true; // aceder a documentos confidenciais
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetUnderContent(1);
                int width = (int)reader.GetPageSize(1).Width;
                int height = (int)reader.GetPageSize(1).Height;
                int middle_w = width / 3;

                //header 
                iTextSharp.text.Image barcode_img_header = iTextSharp.text.Image.GetInstance(barcodeStream2);
                barcode_img_header.SetAbsolutePosition(middle_w, height - commom.height_barcode * 2 + 10);
                barcode_img_header.SetDpi(dpiBarcode, dpiBarcode);
                pdfContentByte.AddImage(barcode_img_header);

                //footer barcode
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
