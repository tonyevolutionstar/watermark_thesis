using System;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class Retificar : Form
    {
        private string file_name;
        private int sizeCircleX;
        private readonly string result_barcode;
        private int id_doc;
        private readonly string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private readonly string infoAnaliseForense = "Procedendo à Análise Forense, aguarde!";
        private string error_readBarcode = "Não consegui ler o código de barras";
        private Commom commom;

        /// <summary>
        /// Retifica um documento com marca de água
        /// </summary>
        /// <param name="file_name"></param>
        /// <param name="sizeCircleX"></param>
        [Obsolete]
        public Retificar(string file_name, int sizeCircleX)
        {
            InitializeComponent();
            commom = new Commom();

            this.file_name = file_name;
            this.sizeCircleX = sizeCircleX;
            commom.Convert_pdf_png(file_name);
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


        private void Retificar_Load(object sender, EventArgs e)
        {
            SQL_connection sql = new SQL_connection();
            if(!String.IsNullOrEmpty(result_barcode))
            {
                string[] resultado = result_barcode.Split(';');
                id_doc = Int32.Parse(resultado[0]);
                string res_doc = sql.Search_document(id_doc);
                if (String.IsNullOrEmpty(res_doc))
                {
                    MessageBox.Show(errorFileDatabase);
                    Close();
                    Dispose();
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
                    Show();
                }
            }
        }


        private void Forense_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(infoAnaliseForense);
            SQL_connection sql = new SQL_connection();
            commom.retificarAnalise(id_doc, sql, file_name, sizeCircleX);
        }

        private void Retificate_FormClosed(object sender, FormClosedEventArgs e)
        {
            string file_png = commom.Get_file_name_using_split(file_name) + ".png";
            if (File.Exists(file_png))
                File.Delete(file_png);
        }
    }
}
