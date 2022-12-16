using System;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{  
    /// <summary>
    /// Menu Principal
    /// </summary>
    public partial class Menu_Principal : Form
    {
        private int size_qrcode = 75;
        string file_name;

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
            Processamento processamento = new Processamento(file_name, size_qrcode);
            processamento.Show(); 
        }

        private void Retificar_btn_Click(object sender, EventArgs e)
        {
            Choose_file();
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
    }
}
