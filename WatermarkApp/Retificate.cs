using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace WatermarkApp
{
    public partial class Retificate : Form
    {
        //messages error
        private string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private string infoAnaliseForense = "Procedendo à Análise Forense, aguarde!";
        private string error_readBarcode = "Não consegui ler o código de barras";
        //----

        private Commom commom;
        private string file_name;
        private readonly string result_barcode;
        private int id_doc;
        private int x_or;
        private int y_or;
        private int prop_x;
        private int prop_y;
        private int diff_x;
        private int diff_y;
        private string img;
        private string rotated_img;

        /// <summary>
        /// Retifica um documento com marca de água
        /// </summary>
        /// <param name="file_name"></param>

        [Obsolete]
        public Retificate(string file_name)
        {
            InitializeComponent();
            commom = new Commom();

            this.file_name = file_name;
            img = commom.Convert_pdf_png(file_name);

            string val = Fix_Rotation();
            string[] values = val.Split('|');
            rotated_img = values[0];


            if (rotated_img.Contains("rotated"))
            {
                string[] file_val = rotated_img.Split(new[] { "_rotated" }, StringSplitOptions.None);
                FileStream sourceStream = new FileStream(file_val[0] + ".pdf", FileMode.Open, FileAccess.Read, FileShare.Read);
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
                iTextSharp.text.Rectangle pageSize = reader.GetPageSizeWithRotation(1);
                sourceStream.Close();
                Document doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream(file_val[0] + "_rotated.pdf", FileMode.Create));
                doc.Open();
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(rotated_img);
                image.SetAbsolutePosition(0, 0);
                image.ScaleToFit(pageSize.Width, pageSize.Height);
                doc.Add(image);
                doc.Close();

                if ((file_val[0] + "_rotated.pdf").Contains("_rotated"))
                {
                    FileSystem.DeleteFile(file_val[0] + ".pdf", UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
                    File.Copy(file_val[0] + "_rotated.pdf", file_val[0] + ".pdf");
                    File.Delete(file_val[0] + "_rotated.png");
                    File.Delete(file_val[0] + "_rotated.pdf");
                    file_name = file_val[0] + ".pdf";
                }
            }

            file_qrcode.src = file_name;
            Controls.Add(file_qrcode);
            result_barcode = commom.Read_barcode(file_name);

            if (result_barcode == "insucesso")
            {
                MessageBox.Show(error_readBarcode);
                Close();
                Dispose();
            }
        }


        private string Fix_Rotation()
        {
            string[] s_doc = img.Split(new[] { ".png" }, StringSplitOptions.None);

            var copy_image = (Bitmap)System.Drawing.Image.FromFile(img);

            //create strips
            var stripCount = 10;

            var compact = new Compact(copy_image, stripCount);

            //find rotation angle
            var stripX1 = 2;//take 3-rd strip
            var stripX2 = 6;//take 7-th strip

            var angle = SkewCalculator.FindRotateAngle(compact, stripX1, stripX2);
            angle = (angle * 180 / Math.PI);//to degrees

            Bitmap rotated = Rotator.Rotate(copy_image, angle);
            rotated.Save(s_doc[0] + "_rotated.png");
            rotated.Dispose();

            return s_doc[0] + "_rotated.png" + "|" + angle;
        }



        private void Retificate_Load(object sender, EventArgs e)
        {
            SQL_connection sql = new SQL_connection();
            if (!String.IsNullOrEmpty(result_barcode))
            {
                string[] resultado = result_barcode.Split(';');

                id_doc = int.Parse(resultado[0]);

                string res_doc = sql.Search_document(id_doc);
                if (String.IsNullOrEmpty(res_doc))
                {
                    MessageBox.Show(errorFileDatabase);
                    Close();
                    Dispose();
                }
                else
                {
                    string barcode_pos = sql.Get_Positions_Barcode(id_doc);
                    string[] val_barcode_pos = barcode_pos.Split(':');
                    // or = original
                    x_or = int.Parse(val_barcode_pos[0]);
                    y_or = int.Parse(val_barcode_pos[1]);

                    string ret_pos_barcode = commom.Return_PositionBarcode(file_name);
                    string[] res_barcode_pos = ret_pos_barcode.Split(':');
                    //Dig = digitalizado
                    int x_dig = int.Parse(res_barcode_pos[0]);
                    int y_dig = int.Parse(res_barcode_pos[1]);

                    diff_x = x_dig - x_or;
                    diff_y = y_dig - y_or;

                    Console.WriteLine($"original barcode position {barcode_pos}, retificar barcode position {ret_pos_barcode}");
                    Console.WriteLine($"diffence between barcode x = {diff_x}, y = {diff_y}");

                    prop_x = x_dig / x_or;
                    prop_y = y_dig / y_or;

                    string[] col_sql = res_doc.Split(';');
                    dct_name.Text = col_sql[0];
                    user.Text = col_sql[1];
                    sigla.Text = col_sql[2];
                    posto.Text = col_sql[3];
                    Controls.Add(dct_name);
                    Controls.Add(user);
                    Controls.Add(sigla);
                    Controls.Add(posto);
                    Show();
                }
            }
        }


        private void Forense_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(infoAnaliseForense);
            SQL_connection sql = new SQL_connection();
            commom.RetificateAnalise(id_doc, sql, file_name, diff_x, diff_y, prop_x, prop_y);
        }

        private void Retificate_FormClosed(object sender, FormClosedEventArgs e)
        {
            string file_png = commom.Get_file_name_using_split(file_name) + ".png";
            if (File.Exists(file_png))
                File.Delete(file_png);
        }
    }
}
