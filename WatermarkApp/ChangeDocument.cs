using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WatermarkApp
{
    public class ChangeDocument
    {
        private readonly string name;
        private readonly string name_without_ex;
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
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(name_without_ex + "_scale_" + s + ".pdf", FileMode.Create));
            doc.Open();
            PdfImportedPage page = writer.GetImportedPage(reader, 1); //page #1
            PdfDictionary pageDict = reader.GetPageN(1);
            pageDict.Put(PdfName.PRINTSCALING, new PdfNumber((float)scalef));

            float yPos = reader.GetPageSize(1).Height - (reader.GetPageSize(1).Height * (float)scalef);
            writer.DirectContent.AddTemplate(page, (float)scalef, 0, 0, (float)scalef, 0, yPos);

            doc.NewPage();
            doc.Close();
            reader.Close();
            writer.Close();
        }
    }
}
