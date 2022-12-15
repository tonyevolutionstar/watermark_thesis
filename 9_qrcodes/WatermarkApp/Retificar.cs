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
        private string file_name;
        private int size_qrcode;
        private string resultado_barcode;
        private int id_doc;

        /// <summary>
        /// Retica um documento com watermark
        /// </summary>
        /// <param name="file_name"></param>
        /// <param name="size_qrcode"></param>
        public Retificar(string file_name, int size_qrcode)
        {
            InitializeComponent();
        
            this.file_name = file_name;
            this.size_qrcode = size_qrcode;
            Convert_pdf_png(file_name);
            file_qrcode.src = file_name;
            Controls.Add(file_qrcode);

            string[] show_doc = file_name.Split(new[] { @"Ficheiros\" }, StringSplitOptions.None);
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string file_name_qrcode = s_doc[0] + ".png";
            resultado_barcode = Read_barcode(file_name_qrcode);
            if(File.Exists(file_name_qrcode))
                File.Delete(file_name_qrcode);

        }

        /// <summary>
        /// le o código de barras
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        private string Read_barcode(string file_name)
        {
            BarcodeResult QRBetterResult = BarcodeReader.QuicklyReadOneBarcode(file_name, BarcodeEncoding.Code128, true);
            if (QRBetterResult != null)
                return QRBetterResult.Value;
            return "insucesso";
        }


        /// <summary>
        /// Converte o ficheiro pdf para png
        /// </summary>
        /// <param name="file_name">Nome do ficheiro</param>
        /// <returns>Nome do ficheiro + ".png"</returns>
        private string Convert_pdf_png(string file_name)
        {
            var dd = System.IO.File.ReadAllBytes(file_name);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1); // Install-Package Freeware.Pdf2Png -Version 1.0.1
            string[] filename = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            System.IO.File.WriteAllBytes(filename[0] + ".png", pngByte);
            return filename[0] + ".png";
        }

        private void Retificar_Load(object sender, EventArgs e)
        {
            SQL_connection sql = new SQL_connection();
            Console.WriteLine(resultado_barcode);
            string[] resultado = resultado_barcode.Split(';');
            id_doc = Int32.Parse(resultado[0]);
            string res_doc = sql.Search_document(id_doc);
            string[] col_sql = res_doc.Split(';');
            // nome_ficheiro, utilizador, sigla_principal, posto_atual
            dct_name.Text = col_sql[0];
            user.Text = col_sql[1];
            sigla.Text = col_sql[2];
            posto.Text = col_sql[3];
            Controls.Add(dct_name);
            Controls.Add(user);
            Controls.Add(sigla);
            Controls.Add(posto);
        }

        private void Forense_btn_Click(object sender, EventArgs e)
        {
            SQL_connection sql = new SQL_connection();
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
        }

  
    }
}
