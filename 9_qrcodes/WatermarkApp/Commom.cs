using System;

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

        public void Convert_pdf_png(string file_name_png)
        {
            var dd = System.IO.File.ReadAllBytes(file_name_png);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            System.IO.File.WriteAllBytes(file_name_png+".p", pngByte);
        }
    }
}
