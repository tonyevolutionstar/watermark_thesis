using System;
using System.Collections.Generic;
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

        private int x_second_barcode;
        private int y_second_barcode;
        private int x_second_barcode_res;
        private int y_second_barcode_res;

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
                    x_second_barcode = int.Parse(val_barcode_pos[4]);
                    y_second_barcode = int.Parse(val_barcode_pos[5]);

                    List<string> res_barcode = commom.Return_PositionBarcode(file_name);
                    string[] res_barcode39 = res_barcode[0].Split(':'); 
                    x_second_barcode_res = int.Parse(res_barcode39[0]);
                    y_second_barcode_res = int.Parse(res_barcode39[1]);
                    
                    string[] res_barcode_pos = res_barcode[1].Split(':');
                    //Dig = digitalizado
                    int x_dig = int.Parse(res_barcode_pos[0]);
                    int y_dig = int.Parse(res_barcode_pos[1]);
                    int x2_dig = int.Parse(res_barcode_pos[2]);
                    int y2_dig = int.Parse(res_barcode_pos[3]);
                    
                    int x_diff_or = x2_or - x_or;
                    int x_diff_dig = x2_dig - x_dig;
                    int y_heigth_or = y_second_barcode - y_or;
                    int y_heigth_dig = y_second_barcode_res - y_dig;
                    int y_diff_or = y2_or - y_or;
                    int y_diff_dig = y2_dig - y_dig;

                    //get dimensions doc 
                    string dimensions = sql.GetDimensionsDoc_db(id_doc);
                    string[] val_dim = dimensions.Split(':');
                    int width_doc_or = int.Parse(val_dim[0]);
                    int height_doc_or = int.Parse(val_dim[1]);
                    int width_bmp_or = int.Parse(val_dim[2]);
                    int heigth_bmp_or = int.Parse(val_dim[3]);

                    commom.GetDimensionsDocument(file_name);
                    commom.GetDimensionsImage(file_name);
                    int width_doc_scan = commom.width;
                    int height_doc_scan = commom.height;
                    int width_bmp_scan = commom.width_bmp;
                    int height_bmp_scan = commom.height_bmp;
                    string scan_dimen = $"{width_doc_scan}:{height_doc_scan}:{width_bmp_scan}:{height_bmp_scan}";

                    int diff_width_doc = width_doc_or - width_doc_scan;
                    int diff_height_doc = height_doc_or - height_doc_scan;
                    int diff_width_bmp = width_bmp_or - width_bmp_scan;
                    int diff_heigth_bmp = heigth_bmp_or - height_bmp_scan;
                    string diff_dimen = $"{diff_width_doc}:{diff_height_doc}:{diff_width_bmp}:{diff_heigth_bmp}";

                    diff_x = x_dig - x_or;
                    diff_y = y_diff_dig - y_diff_or;
                    double prop_x = (double) x_diff_dig / x_diff_or;
                    double prop_y = (double) y_diff_dig / y_diff_or;

                    if (!file_name.Contains("scan"))
                        diff_y = 0;
                    Console.WriteLine($"Original pos {barcode_pos}, digital pos {res_barcode[0]} | {res_barcode[1]}"); 
                    Console.WriteLine($"Diference x {diff_x}, diference y {diff_y/2}"); 
                    Console.WriteLine($"X original {x_diff_or}, x retificar {x_diff_dig}");
                    Console.WriteLine($"Y original {y_diff_or}, y retificar {prop_y}");
                    Console.WriteLine($"prop x {prop_x}, prop y {y_diff_dig}");
                    Console.WriteLine($"original dim {dimensions}, Scan positions {scan_dimen}, diff {diff_dimen}");

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
