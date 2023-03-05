using System;
using System.Windows.Forms;

namespace WatermarkApp
{  
    public partial class Principal_Menu : Form
    {
        string file_name;
        private readonly string errorFile_empty = "Por favor escolha um ficheiro";
        private readonly string errorFile_without_watermark = "O ficheiro que selecionou ainda não tem marca de água";
        private readonly string errorFileType = "O ficheiro que selecionou é de formato imagem ou não é de formato pdf";
        private readonly string errorFile_with_watermark = "O ficheiro que selecionou já foi processado";
       
        private Commom commom = new Commom();


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

        private void Retificate_btn_Click(object sender, EventArgs e)
        {
            if (Choose_file())
            {
                string file_name_without_dir = commom.Get_file_name_without_directory(file_name);

                if (file_name_without_dir.Contains(commom.extension_watermark))
                {
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
