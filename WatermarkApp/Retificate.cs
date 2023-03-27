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
        private double coef_x;
        private double coef_y;  
  
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
                    int y2_or = int.Parse(val_barcode_pos[3]);
           
                    string ret_pos_barcode = commom.Return_PositionBarcode(file_name);
                    string[] res_barcode_pos = ret_pos_barcode.Split(':');
                    //Dig = digitalizado
                    int x_dig = int.Parse(res_barcode_pos[0]);
                    int y_dig = int.Parse(res_barcode_pos[1]);
                    int x2_dig = int.Parse(res_barcode_pos[2]);   
                    int y2_dig = int.Parse(res_barcode_pos[3]);
                    int x_diff_or = x2_or - x_or;
                    int x_diff_dig = x2_dig - x_dig;
                    int y_diff_dig = y2_dig - y_dig;
                    int y_diff_or = y2_or - y_or;

                    diff_x = x_dig - x_or;
                    diff_y = y_dig - y_or;
                    Console.WriteLine($"Original pos {barcode_pos}, digital pos {ret_pos_barcode}"); 
                    Console.WriteLine($"Diference x {diff_x}, diference y {diff_y}");
                    

                    coef_x = (double) x_diff_dig / x_diff_or;
                    coef_y = (double) y_diff_dig / y_diff_or;
                  
                    Console.WriteLine($"X original {x_diff_or}, x retificar {x_diff_dig}");
                    Console.WriteLine($"Coeficient x {coef_x}, Coeficient y {coef_y}");
               
                    
                    string img_file = commom.Convert_pdf_png(file_name);
                    Font drawFont = new Font("Arial", 10);
                    SolidBrush drawBrush = new SolidBrush(Color.Blue);
                    commom.GetDimensionsDocument(file_name);


                    using (Bitmap bmp = new Bitmap(img_file))
                    {
                        using(Graphics g = Graphics.FromImage(bmp))
                        {
                            g.DrawString("p1_d_l_u", drawFont, drawBrush, x_dig * bmp.Width / commom.width, y_dig * bmp.Height/commom.height);
                            g.DrawString("p1_d_l_b", drawFont, drawBrush, x_dig * bmp.Width / commom.width, y2_dig * bmp.Height / commom.height);
                            g.DrawString("p1_d_r_u", drawFont, drawBrush, x2_dig * bmp.Width / commom.width, y_dig * bmp.Height / commom.height);
                            g.DrawString("p1_d_r_b", drawFont, drawBrush, x2_dig * bmp.Width / commom.width, y2_dig * bmp.Height / commom.height);
                        }

                        bmp.Save(commom.files_dir + @"\test.png");
                    }
                   

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
            commom.RetificateAnalise(id_doc, sql, file_name, diff_x, diff_y, coef_x, coef_y);
        }

        private void Retificate_FormClosed(object sender, FormClosedEventArgs e)
        {
            string name_file = commom.Get_file_name_using_split(file_name);
            string file_rotated = name_file + "_rotated.png";
            string img_file = name_file + ".png";
            if (File.Exists(file_rotated))
                File.Delete(file_rotated);  
            if (File.Exists(img_file))
                File.Delete(img_file);
        }
    }
}
