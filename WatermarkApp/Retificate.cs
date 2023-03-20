using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class Retificate : Form
    {
        private string errorFileDatabase = "O ficheiro que selecionou não foi aprovado nem aceite na base de dados";
        private string infoAnaliseForense = "Procedendo à Análise Forense, aguarde!";
        private string error_readBarcode = "Não consegui ler o código de barras";

        private Commom commom;
        private string file_name;
        private readonly string result_barcode;
        private int id_doc;
        private int x_or;
        private int y_or;

        private int diff_x;
        private int diff_y;
  
        private TrackerServices tracker = new TrackerServices(); 

        /// <summary>
        /// Retifica um documento com marca de água
        /// </summary>
        /// <param name="file_name"></param>

        [Obsolete]
        public Retificate(string file_name)
        {
            tracker.WriteFile("---------- Retificar -----------");
            tracker.WriteFile($"retificação do ficheiro {file_name} a iniciar");
            InitializeComponent();
            commom = new Commom();
            this.file_name = file_name;

            file_qrcode.src = file_name;
            Controls.Add(file_qrcode);
            result_barcode = commom.Read_barcode(file_name);

            if (result_barcode == commom.errorReadBarcode)
            {
                tracker.WriteFile($"retificação do ficheiro {file_name} falhou - {error_readBarcode}");
                MessageBox.Show(error_readBarcode);
                Close();
                Dispose();
            }
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
                    int x2_or = int.Parse(val_barcode_pos[2]);

                    string img = commom.Convert_pdf_png(file_name);
                    commom.GetDimensionsDocument(file_name);
                        
                    string ret_pos_barcode = commom.Return_PositionBarcode(file_name);
                    string[] res_barcode_pos = ret_pos_barcode.Split(':');
                    //Dig = digitalizado
                    int x_dig = int.Parse(res_barcode_pos[0]);
                    int y_dig = int.Parse(res_barcode_pos[1]);
                    int x2_dig = int.Parse(res_barcode_pos[2]);
                    

                    using (Bitmap bmp = new Bitmap(img))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            SolidBrush drawBrush = new SolidBrush(Color.Chocolate);
                            Font drawFont = new Font("Arial", 10);
                            int p_x = x_dig * bmp.Width / commom.width; 
                            int p_y = y_dig * bmp.Height / commom.height; 
                            int p2_x = x2_dig * bmp.Width / commom.width;
                            Point p = new Point(p_x, p_y);
                            Point p2 = new Point(p2_x, p_y);
                            g.DrawString("p1", drawFont, drawBrush, p);
                            g.DrawString("p2", drawFont, drawBrush, p2);
                        }
                        string[] filename = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
                        bmp.Save(filename[0] + "_pos_barcode.png", System.Drawing.Imaging.ImageFormat.Png);
                        bmp.Dispose();
                    }
                    
                    diff_x = x_or - x_dig;
                    diff_y = y_or - y_dig;

                    Console.WriteLine($"original barcode position {barcode_pos}, retificar barcode position {ret_pos_barcode}");
                    Console.WriteLine($"diffence between barcode x = {diff_x}, y = {diff_y}");

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
            commom.RetificateAnalise(id_doc, sql, file_name, diff_x, diff_y);
        }

    
    }
}
