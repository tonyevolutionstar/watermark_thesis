using Aspose.Words;
using IronBarCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    /// <summary>
    /// Menu retificar
    /// </summary>
    public partial class Retificar : Form
    {
        private readonly string file_name;
        private readonly int size_qrcode;
        private readonly string resultado_barcode;
        private int id_doc;
        private readonly string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private readonly string infoAnaliseForense = "Processedendo à Análise Forense, aguarde!";


        /// <summary>
        /// Retica um documento com watermark
        /// </summary>
        /// <param name="file_name"></param>
        /// <param name="size_qrcode"></param>
        [Obsolete]
        public Retificar(string file_name, int size_qrcode)
        {
            InitializeComponent();
            this.file_name = file_name;
            this.size_qrcode = size_qrcode;
            Convert_pdf_png(file_name);
            file_qrcode.src = file_name;
            Controls.Add(file_qrcode);
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string file_name_qrcode = s_doc[0] + ".png";

            resultado_barcode = Read_barcode(file_name_qrcode);
            if (File.Exists(file_name_qrcode))
                File.Delete(file_name_qrcode);
        }

        public void Convert_pdf_png(string file_name_png)
        {
            string[] filename = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            var dd = System.IO.File.ReadAllBytes(file_name_png);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            System.IO.File.WriteAllBytes(filename[0] + ".png", pngByte);
        }


        [Obsolete]
        private string Read_barcode(string file_name)
        {
            BarcodeResult QRBetterResult = BarcodeReader.QuicklyReadOneBarcode(file_name, BarcodeEncoding.Code128, true);
            if (QRBetterResult != null)
                return QRBetterResult.Value;
            return "insucesso";
        }


        private void Retificar_Load(object sender, EventArgs e)
        {
            SQL_connection sql = new SQL_connection();
            string[] resultado = resultado_barcode.Split(';');
            id_doc = Int32.Parse(resultado[0]);
            string res_doc = sql.Search_document(id_doc);
            if (String.IsNullOrEmpty(res_doc))
            {
                MessageBox.Show(errorFileDatabase);
                this.Close();
                this.Dispose();
            } 
            else
            {
                string[] col_sql = res_doc.Split(';');
                dct_name.Text = col_sql[0];
                user.Text = col_sql[1];
                sigla.Text = col_sql[2];
                posto.Text = col_sql[3];
                Controls.Add(dct_name);
                Controls.Add(user);
                Controls.Add(sigla);
                Controls.Add(posto);
                this.Show();
            }
        }


        private void Forense_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(infoAnaliseForense);
            SQL_connection sql = new SQL_connection();
            List<string> returnlist = sql.Get_Values_Analise_Forense(id_doc);
            AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name, size_qrcode);
            string img = auxFunc.DrawImage(returnlist, file_name);

            string[] filename = img.Split(new[] { ".png" }, StringSplitOptions.None);

            var doc = new Document();
            var builder = new DocumentBuilder(doc);
            builder.InsertImage(img);
            doc.Save(filename[0] + ".pdf");

            string[] file = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);

            AnaliseForenseForm form = new AnaliseForenseForm(filename[0] + ".pdf");
            form.Show();
          
            if (File.Exists(file[0] + ".png"))
                File.Delete(file[0] + ".png");
            if (File.Exists(filename[0] + ".png"))
                File.Delete(filename[0] + ".png");
        }
    }
}
