using IronBarCode;
using iTextSharp.text.pdf;
using System;
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


        /// <summary>
        /// posição que vai ser inserido o qrcode no ficheiro
        /// </summary>
     
        private int version = 2;
        private int size_qrcode; //Width == height para ser quadrado
        private int range; 

    
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
            id_doc = id;
            this.range = range;
        }


        /// <summary>
        /// Guarda uma imagem do qrcode gerado com base nas caracteristicas do ficheiro
        /// Estrutura do qrcode é version;id_doc;info_char;range
        /// <param name="i">numero identificador do qrcode, max até 9</param> 
        /// <returns>imagem qrcode</returns>
        /// </summary>

        public void Generate_qrcode(int i)
        {
            if(version == 2)
            {
                string data_qrcode = version + ";" + id_doc + ";" + range;

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
    }
}
