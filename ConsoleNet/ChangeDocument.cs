using System;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ConsoleNet
{
    public class ChangeDocument
    {
        private readonly string name;
        private readonly string name_without_ex;
        private string file_name;

        public ChangeDocument(string name)
        {
            this.name = name;
            string[] filename = name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            name_without_ex = filename[0];
        }

        public void Scale(decimal scalef)
        {
            int s = Convert.ToInt16(scalef * 100);
            PdfReader reader = new PdfReader(name);
            PdfReader.unethicalreading = true; // aceder a documentos confidenciais
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc,new FileStream(name_without_ex + "_scale_" + s + ".pdf", FileMode.Create));
            doc.Open();
            PdfImportedPage page = writer.GetImportedPage(reader, 1);
            PdfDictionary pageDict = reader.GetPageN(1);
            pageDict.Put(PdfName.PRINTSCALING, new PdfNumber((float)scalef));

            float yPos = reader.GetPageSize(1).Height - (reader.GetPageSize(1).Height * (float)scalef); 
            writer.DirectContent.AddTemplate(page, (float)scalef, 0, 0, (float)scalef, 0, yPos);

            doc.NewPage();
            doc.Close();
            reader.Close();
            writer.Close();
        }

        public void RotateDoc(string img_file,int angle)
        {
            string[] f = img_file.Split(new[] { ".png" }, StringSplitOptions.None);
            file_name = f[0];
            string output = f[0] + "_rotate_" + angle + ".png";
            
            Bitmap bmp = new Bitmap(img_file);
            Bitmap img = new Bitmap(bmp.Width, bmp.Height); 

            Graphics g = Graphics.FromImage(img);
            g.TranslateTransform(bmp.Width / 2, bmp.Height / 2); 
            g.RotateTransform(angle);
            g.DrawImage(bmp, -bmp.Width / 2, -bmp.Height / 2, bmp.Width, bmp.Height);
            g.Dispose();
            img.Save(output);
            bmp.Dispose();
            img.Dispose();
        }

        public void AddImage_pdf(int angle)
        {
            string rot_file = file_name + "_rotate_" + angle;
            string outputPDF = rot_file + ".pdf";
            string outputPNG = rot_file + ".png";
            Bitmap bmp = new Bitmap(outputPNG);
            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(outputPDF, FileMode.Create));
            doc.Open();
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(outputPNG);
            image.SetDpi(300, 300);
            image.SetAbsolutePosition(0, 0); // canto superior esquerdo
            image.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
            doc.Add(image);
            doc.Close();
            bmp.Dispose();
            if(File.Exists(outputPNG))
                File.Delete(outputPNG);
        }
    }
}
