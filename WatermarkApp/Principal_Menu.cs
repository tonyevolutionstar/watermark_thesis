using System;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace WatermarkApp
{  
    public partial class Principal_Menu : Form
    {
        string file_name;
        private string errorFile_empty = "Por favor escolha um ficheiro";
        private string errorFile_without_watermark = "O ficheiro que selecionou ainda não tem marca de água";
        private string errorFileType = "O ficheiro que selecionou é de formato imagem ou não é de formato pdf";
        private string errorFile_with_watermark = "O ficheiro que selecionou já foi processado";
       
        private Commom commom = new Commom();
        double angle;
        TrackerServices tracker = new TrackerServices();

        public Principal_Menu()
        {
            InitializeComponent();
        }


        private bool Choose_file()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = commom.files;

            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                file_name = ofd.FileName;
                if (!System.IO.File.Exists(file_name))
                {
                   return false;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                return false;
            }
            return true;
        }

        private void Process_btn_Click(object sender, EventArgs e)
        {
            if (Choose_file())
            {
                string file_name_without_dir = commom.Get_file_name_without_directory(file_name);

                if (file_name.Contains(".png"))
                {
                    MessageBox.Show(errorFileType);
                }
                else
                {
                    if (file_name_without_dir.Contains(commom.extension_watermark) || file_name_without_dir.Contains(commom.extension_integrity))
                    {
                        MessageBox.Show(errorFile_with_watermark);
                    }
                    else
                    {
                        Process processamento = new Process(file_name);
                        processamento.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show(errorFile_empty);
            }
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
                    string[] file_val = rotated_img.Split(new[] { "_rotated" }, StringSplitOptions.None);
             
                    Document doc = new Document();
                    PdfWriter.GetInstance(doc, new FileStream(file_val[0] + "_rotated.pdf", FileMode.Create));
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

        private string Fix_Rotation()
        {
            string img = commom.Convert_pdf_png(file_name);
            string[] s_doc = img.Split(new[] { ".png" }, StringSplitOptions.None);

            var copy_image = (Bitmap)System.Drawing.Image.FromFile(img);
            int stripCount = 10;
            Compact compact = new Compact(copy_image, stripCount);

            //find rotation angle
            int stripX1 = 2;//take 3-rd strip
            int stripX2 = 6;//take 7-th strip

            double angle = SkewCalculator.FindRotateAngle(compact, stripX1, stripX2);
            angle = (angle * 180 / Math.PI);//to degrees

            Bitmap rotated = Rotator.Rotate(copy_image, angle);
            rotated.Save(s_doc[0] + "_rotated.png");
            rotated.Dispose();

            return s_doc[0] + "_rotated.png" + "|" + angle;
        }



        private void Retificate_btn_Click(object sender, EventArgs e)
        {
            if (Choose_file())
            {
                string file_name_without_dir = commom.Get_file_name_without_directory(file_name);

                if (file_name_without_dir.Contains(commom.extension_watermark))
                {
                    if(file_name.Contains("scan"))
                    {
                        tracker.WriteFile("verificar se o ficheiro scan está torto");
                        ChangeFile_Rotated();
                    }

                    Retificate retificar = new Retificate(file_name);
                    try
                    {
                        retificar.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show(errorFile_without_watermark);
                }
            }
            else
            {
                MessageBox.Show(errorFile_empty);
            }
        }
    }
}
