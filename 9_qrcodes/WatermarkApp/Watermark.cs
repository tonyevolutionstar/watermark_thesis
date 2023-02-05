using IronBarCode;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using System.IO;

namespace WatermarkApp
{
    /// <summary>
    ///  Classe destinada a fazer a marca de agua no ficheiro
    /// </summary>
    public class Watermark
    {
        private string filename = "";
        private int id_doc;
        string circleX_img = @"\number_qrcode\X_resized.png";

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

        
        public void Add_watermark_pdf(string positions, string date_time)
        {
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            using (Stream inputPdfStream = new FileStream(filename + ".pdf", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream2 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream3 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream4 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream5 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream6 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream7 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream8 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream x_stream9 = new FileStream(partialPath + circleX_img, FileMode.Open, FileAccess.Read, FileShare.Read))
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

                string[] positions_qrcode = positions.Split('|');
                int[] indices = { 6, 7, 8, 3, 4, 5, 0, 1, 2};

                iTextSharp.text.Image x_img = iTextSharp.text.Image.GetInstance(x_stream);
                string[] qrcode = positions_qrcode[6].Split(',');
                x_img.SetAbsolutePosition(Int16.Parse(qrcode[0]), Int16.Parse(qrcode[1]));
                pdfContentByte.AddImage(x_img);

                iTextSharp.text.Image x_img2 = iTextSharp.text.Image.GetInstance(x_stream2);
                string[] qrcode2 = positions_qrcode[7].Split(',');
                x_img2.SetAbsolutePosition(Int16.Parse(qrcode2[0]), Int16.Parse(qrcode2[1]));
                pdfContentByte.AddImage(x_img2);

                iTextSharp.text.Image x_img3 = iTextSharp.text.Image.GetInstance(x_stream3);
                string[] qrcode3 = positions_qrcode[8].Split(',');
                x_img3.SetAbsolutePosition(Int16.Parse(qrcode3[0]), Int16.Parse(qrcode3[1]));
                pdfContentByte.AddImage(x_img3);

                iTextSharp.text.Image x_img4 = iTextSharp.text.Image.GetInstance(x_stream4);
                string[] qrcode4 = positions_qrcode[3].Split(',');
                x_img4.SetAbsolutePosition(Int16.Parse(qrcode4[0]), Int16.Parse(qrcode4[1]));
                pdfContentByte.AddImage(x_img4);

                iTextSharp.text.Image x_img5 = iTextSharp.text.Image.GetInstance(x_stream5);
                string[] qrcode5 = positions_qrcode[4].Split(',');
                x_img5.SetAbsolutePosition(Int16.Parse(qrcode5[0]), Int16.Parse(qrcode5[1]));
                pdfContentByte.AddImage(x_img5);

                iTextSharp.text.Image x_img6 = iTextSharp.text.Image.GetInstance(x_stream6);
                string[] qrcode6 = positions_qrcode[5].Split(',');
                x_img6.SetAbsolutePosition(Int16.Parse(qrcode6[0]), Int16.Parse(qrcode6[1]));
                pdfContentByte.AddImage(x_img6);

                iTextSharp.text.Image x_img7 = iTextSharp.text.Image.GetInstance(x_stream7);
                string[] qrcode7 = positions_qrcode[0].Split(',');
                x_img7.SetAbsolutePosition(Int16.Parse(qrcode7[0]), Int16.Parse(qrcode7[1]));
                pdfContentByte.AddImage(x_img7);

                iTextSharp.text.Image x_img8 = iTextSharp.text.Image.GetInstance(x_stream8);
                string[] qrcode8 = positions_qrcode[1].Split(',');
                x_img8.SetAbsolutePosition(Int16.Parse(qrcode8[0]), Int16.Parse(qrcode8[1]));
                pdfContentByte.AddImage(x_img8);

                iTextSharp.text.Image x_img9 = iTextSharp.text.Image.GetInstance(x_stream9);
                string[] qrcode9 = positions_qrcode[2].Split(',');
                x_img9.SetAbsolutePosition(Int16.Parse(qrcode9[0]), Int16.Parse(qrcode9[1]));
                pdfContentByte.AddImage(x_img9);

                iTextSharp.text.Image barcode_img = iTextSharp.text.Image.GetInstance(barcodeStream);
                barcode_img.SetAbsolutePosition(middle_w, 15);
                barcode_img.SetDpi(100, 100);
                pdfContentByte.AddImage(barcode_img);

                stamper.Close();
                stamper.Dispose();
                reader.Dispose();
                inputPdfStream.Dispose();
                x_stream.Dispose();
            }
        }
    }
}
