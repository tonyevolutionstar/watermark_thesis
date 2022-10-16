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
            using (Stream barcodeStream = new FileStream(filename + "_barcode.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(filename + "_qrcode.pdf", FileMode.Create, FileAccess.Write, FileShare.Write))  
            {
                PdfReader reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true;
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetUnderContent(1);
                int width = (int)reader.GetPageSize(1).Width;
                int height = (int)reader.GetPageSize(1).Height;
                float m = width / 3;
                int middle_w = (int)Math.Round(m);
                string[] positions_qrcode = positions.Split('|');
                iTextSharp.text.Image barcode_img = iTextSharp.text.Image.GetInstance(barcodeStream);
                barcode_img.SetAbsolutePosition(middle_w, 5);
                for (int i = 0; i < positions_qrcode.Length; i++)
                {
                    using (Stream qrcodeStream_x = new FileStream(filename + "_qrcode_" + (i+1) + ".png", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        string[] qrcodex = positions_qrcode[i].Split(',');
                        iTextSharp.text.Image qr_code_img_x = iTextSharp.text.Image.GetInstance(qrcodeStream_x);
                        qr_code_img_x.SetAbsolutePosition(Int16.Parse(qrcodex[0]), Int16.Parse(qrcodex[1]));
                        pdfContentByte.AddImage(qr_code_img_x);
                        pdfContentByte.AddImage(barcode_img);                
                    }
                }

                stamper.Close();
                stamper.Dispose();
                reader.Dispose();
                inputPdfStream.Dispose();
            }     
        }
    }
}
