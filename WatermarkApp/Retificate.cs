using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class Retificate : Form
    {
        private string file_name;
        private readonly string result_barcode;
        private int id_doc;
        private readonly string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private readonly string infoAnaliseForense = "Procedendo à Análise Forense, aguarde!";
        private string error_readBarcode = "Não consegui ler o código de barras";
        private Commom commom;

        private int difference_x;
        private int difference_y;
        private int angle;
        private int prop_x;
        private int prop_y;
        private string img;
        private int width;
        private int height;

        /// <summary>
        /// Retifica um documento com marca de água
        /// </summary>
        /// <param name="file_name"></param>

        [Obsolete]
        public Retificate(string file_name)
        {
            InitializeComponent();
            commom = new Commom();
            commom.GetDimensionsDocument(file_name);
            width = commom.width;
            height = commom.height;

            this.file_name = file_name;
            img = commom.Convert_pdf_png(file_name);
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

   
        private int Calculate_angle(int x1, int y1, int x2, int y2)
        {
            angle = Convert.ToInt16(Math.Atan2(y2 - y1, x2 - x1) * (180 / Math.PI));
            return angle;
        }

        private void Retificate_Load(object sender, EventArgs e)
        {
            SQL_connection sql = new SQL_connection();
            if(!String.IsNullOrEmpty(result_barcode))
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
                    Bitmap bmp = new Bitmap(img);
                    string barcode_pos = sql.Get_Positions_Barcode(id_doc);
                    string[] val_barcode_pos = barcode_pos.Split(':');
                    // or = original
                    int x_or = int.Parse(val_barcode_pos[0]);
                    int y_or = int.Parse(val_barcode_pos[1]);

                    string ret_pos_barcode = commom.Return_PositionBarcode(file_name);
                    string[] res_barcode_pos = ret_pos_barcode.Split(':');
                    //Dig = digitalizado
                    int x_dig = int.Parse(res_barcode_pos[0]) * bmp.Width / width;
                    int y_dig = int.Parse(res_barcode_pos[1]) * bmp.Height / height;


                    // significa que o ficheiro de entrada não tem as mesmas coordenadas do que o ficheiro original,
                    // ou seja o ficheiro é digitalizado
                    if (!ret_pos_barcode.Equals(barcode_pos))
                    {
                        difference_x = x_or - x_dig;
                        difference_y = y_or - y_dig;
                    }

                    Console.WriteLine($"original barcode position {barcode_pos}, retificar barcode position {ret_pos_barcode}");
                    Console.WriteLine($"difference x {difference_x}, difference y {difference_y}");
                    angle = Calculate_angle(x_or, y_or, x_dig, y_dig);

                    prop_x = x_or / x_dig;
                    prop_y = y_or / y_dig;
                   
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
                    bmp.Dispose();
                }
            }
        }


        private void Forense_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show(infoAnaliseForense);
            SQL_connection sql = new SQL_connection();
            commom.RetificateAnalise(id_doc, sql, file_name, difference_x, difference_y, angle);
        }

        private void Retificate_FormClosed(object sender, FormClosedEventArgs e)
        {
            string file_png = commom.Get_file_name_using_split(file_name) + ".png";
            if (File.Exists(file_png))
                File.Delete(file_png);
        }
    }
}
