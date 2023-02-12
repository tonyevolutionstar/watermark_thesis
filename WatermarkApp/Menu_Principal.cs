using System;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{  
    /// <summary>
    /// 
    /// </summary>
    public partial class Menu_Principal : Form
    {
        private readonly int sizeCircleX = 75;
        string file_name;
        private readonly string errorFile_empty = "Por favor escolha um ficheiro";
        private readonly string errorFile_without_watermark = "O ficheiro que selecionou ainda não tem marca de água";
        private readonly string errorFileType = "O ficheiro que selecionou é de formato imagem ou não é de formato pdf";
        private readonly string errorFile_with_watermark = "O ficheiro que selecionou já foi processado";
        private Commom commom = new Commom();

        /// <summary>
        /// Menu principal programa
        /// </summary>
        public Menu_Principal()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e) { }

        private bool Choose_file()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            ofd.InitialDirectory = Path.Combine(partialPath, @"Ficheiros\");

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

        private void Processar_btn_Click(object sender, EventArgs e)
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
                    if (file_name_without_dir.Contains("watermark"))
                    {
                        MessageBox.Show(errorFile_with_watermark);
                    }
                    else
                    {
                        Processamento processamento = new Processamento(file_name, sizeCircleX);
                        processamento.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show(errorFile_empty);
            }
        }

        private void Retificar_btn_Click(object sender, EventArgs e)
        {
            if (Choose_file())
            {
                string file_name_without_dir = commom.Get_file_name_without_directory(file_name);

                if (file_name_without_dir.Contains("watermark"))
                {
                    Retificar retificar = new Retificar(file_name, sizeCircleX);
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

        private void RetificarManual_btn_Click(object sender, EventArgs e)
        {
            if (Choose_file())
            {
                string file_name_without_dir = commom.Get_file_name_without_directory(file_name);

                if (!file_name.Contains(".pdf"))
                {
                    MessageBox.Show("Extensão de ficheiro não permitida");
                }
                else
                {
                    if (file_name_without_dir.Contains("watermark"))
                    {
                        RetificarManual retificarManual = new RetificarManual(file_name, sizeCircleX);
                        try
                        {
                            retificarManual.Show();
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
            }
            else
            {
                MessageBox.Show(errorFile_empty);
            }

        }
    }
}
