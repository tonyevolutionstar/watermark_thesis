using System;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class Retificar : Form
    {
        private string file_name;
        private readonly int sizeCircleX;
        private readonly string resultado_barcode;
        private int id_doc;
        private readonly string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private readonly string infoAnaliseForense = "Processedendo à Análise Forense, aguarde!";
        private string watermark_file;


        /// <summary>
        /// Retica um documento com watermark
        /// </summary>
        /// <param name="file_name"></param>
        /// <param name="size_qrcode"></param>
        [Obsolete]
        public Retificar(string file_name, int size_qrcode)
        {
            InitializeComponent();
            Commom commom = new Commom();

            this.file_name = file_name;
            sizeCircleX = size_qrcode;
            commom.Convert_pdf_png(file_name);
            file_qrcode.src = file_name;
            Controls.Add(file_qrcode);
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            watermark_file = s_doc[0] + ".png";
            resultado_barcode = commom.Read_barcode(watermark_file);
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
            Commom commom = new Commom();
            commom.retificarAnalise(id_doc, sql, file_name, sizeCircleX);
        }
    }
}
