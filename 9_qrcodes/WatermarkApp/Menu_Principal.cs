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
        private readonly int size_qrcode = 75;
        string file_name;
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

        private void Choose_file()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            ofd.InitialDirectory = Path.Combine(partialPath, @"Ficheiros\");
            if (DialogResult.OK == ofd.ShowDialog())
            {
                file_name = ofd.FileName;
            }
        }

        private void Processar_btn_Click(object sender, EventArgs e)
        {
            Choose_file();
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
                    Processamento processamento = new Processamento(file_name, size_qrcode);
                    processamento.Show();
                }
             
            }
            
        }

        [Obsolete]
        private void Retificar_btn_Click(object sender, EventArgs e)
        {
            Choose_file();
           
            string file_name_without_dir = commom.Get_file_name_without_directory(file_name); 
       
            if (file_name_without_dir.Contains("watermark"))
            {
                Retificar retificar = new Retificar(file_name, size_qrcode);
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
    }
}
