using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace TestesApp
{
    public class PrincipalMenu
    {
        string file_name;
    
        private readonly string errorFile_without_watermark = "O ficheiro que selecionou ainda não tem marca de água";
        private readonly string errorFileType = "O ficheiro que selecionou é de formato imagem ou não é de formato pdf";
        private readonly string verify_scan_angle = "Verificar se o ficheiro scan está torto";

        private readonly Commom commom = new ();
        double angle;
        readonly TrackerServices tracker = new ();
        private readonly string file_name_without_dir;
        Process proc;
        Retificate ret;

        public PrincipalMenu(string file_name, string action)
        {
            this.file_name = file_name;
            if (action.Equals("process"))
                proc = new Process(file_name);
            else if (action.Equals("retificate"))
                RetificateF();
            file_name_without_dir = commom.Get_file_name_without_directory(file_name);
        }


   
        private void RetificateF()
        {
            Console.WriteLine("retificate");
        }

        private string Fix_Rotation()
        {
            string img = commom.Convert_pdf_png(file_name);
            string[] s_doc = img.Split(new[] { ".png" }, StringSplitOptions.None);

            var copy_image = (Bitmap)System.Drawing.Image.FromFile(img);
            int stripCount = 10; // se o scan nao ter posições ou estiver muito torto alterar para 30
            var compact = new Compact(copy_image, stripCount);

            //find rotation angle
            int stripX1 = 2;//take 3-rd strip
            int stripX2 = 6;//take 7-th strip

            double angle = SkewCalculator.FindRotateAngle(compact, stripX1, stripX2);
            angle = (angle * 180 / Math.PI);//to degrees
            if (angle != 0)
            {
                var rotated = Rotator.Rotate(copy_image, angle);
                rotated.Save(s_doc[0] + "_rotated.png");
            }

            return s_doc[0] + "_rotated.png" + "|" + angle;
        }



        private void ChangeFile_Rotated()
        {
            string val = Fix_Rotation();
            string[] values = val.Split('|');
            string rotated_img = values[0];
            angle = double.Parse(values[1]);

            Console.WriteLine($"angle = {angle}");

            if (angle != 0.0)
            {
                tracker.WriteFile("ficheiro scan está torto");

                if (rotated_img.Contains("rotated"))
                {
                    string[] file_val = rotated_img.Split(new[] { "_scan" }, StringSplitOptions.None);
                    Document doc = new Document(new iTextSharp.text.Rectangle(0, 0, 578, 823));
                    PdfWriter.GetInstance(doc, new FileStream(file_val + "_rotated.pdf", FileMode.Create));
                    doc.Open();
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(rotated_img);
                    image.SetDpi(300, 300);
                    image.SetAbsolutePosition(0, 0); // canto superior esquerdo
                    image.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                    doc.Add(image);
                    doc.Close();

                    FileSystem.DeleteFile(file_val[0] + ".pdf", UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                    FileSystem.CopyFile(file_val[0] + "_rotated.pdf", file_val[0] + ".pdf", UIOption.OnlyErrorDialogs);
                    File.Delete(file_val[0] + "_rotated.pdf");
                    FileSystem.DeleteFile(file_val[0] + "_rotated.png", UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                    FileSystem.DeleteFile(file_val[0] + ".png", UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);

                    tracker.WriteFile("ficheiro scan composto");
                }
            }

            tracker.WriteFile("ficheiro scan está direito");
        }


    }
}
