using Aspose.Words;
using IronBarCode;
using System;
using System.Collections.Generic;
using System.IO;

namespace WatermarkApp
{
    public class Commom
    {   
        /// <summary>
        /// Commom functions between classes
        /// </summary>
        public Commom(){}

        public string Get_file_name_using_split(string file_name)
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            return s_doc[0];
        }


        public string Get_file_name_without_directory(string file_name)
        {
            string[] show_doc = file_name.Split(new[] { @"Ficheiros\" }, StringSplitOptions.None);
            string[] file = show_doc[1].Split(new[] { ".pdf" }, StringSplitOptions.None);
            
            return file[0];
        }


        public string Convert_pdf_png(string file_name_png)
        {
            var dd = System.IO.File.ReadAllBytes(file_name_png);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            string[] filename = file_name_png.Split(new[] { ".pdf" }, StringSplitOptions.None);

            System.IO.File.WriteAllBytes(filename[0]+ ".png", pngByte);
            return filename[0] + ".png";
        }

        public string Read_barcode(string file_name)
        {
            BarcodeResult BarCodeResult = BarcodeReader.QuicklyReadOneBarcode(file_name, BarcodeEncoding.Code128, true);
            if (BarCodeResult != null)
                return BarCodeResult.Value;
            return "insucesso";
        }

        public void retificarAnalise(int id_doc, SQL_connection sql, string file_name, int size_qrcode)
        {
            List<string> returnlist = sql.Get_Values_Analise_Forense(id_doc);
            AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name, size_qrcode);
            string img = auxFunc.DrawImage(returnlist, file_name);

            string[] filename = img.Split(new[] { ".png" }, StringSplitOptions.None);

            var doc = new Document();
            var builder = new DocumentBuilder(doc);
            builder.InsertImage(img);
            doc.Save(filename[0] + ".pdf");

            AnaliseForenseForm form = new AnaliseForenseForm(filename[0] + ".pdf");
            form.Show();
           
            if (File.Exists(filename[0] + ".png"))
                File.Delete(filename[0] + ".png");
        }
    }
}
