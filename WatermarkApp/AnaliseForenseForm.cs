using System;
using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class AnaliseForenseForm : Form
    {
        private string file_name_png;

        /// <summary>
        /// Mostrar resultado da análise forense através de um documento criado com as letras que se devia ter em azul
        /// </summary>
        /// <param name="file_name"></param>
        public AnaliseForenseForm(string file_name)
        {
            InitializeComponent();
            axAcroPDF1.src = file_name;
            Commom commom = new Commom();
            file_name_png = commom.Get_file_name_using_split(file_name) + ".png";
            Controls.Add(axAcroPDF1);
        }

        private void AnaliseForenseForm_Shown(object sender, System.EventArgs e)
        {
            string[] s_doc = file_name_png.Split(new[] { "_integrity" }, StringSplitOptions.None);

            if (System.IO.File.Exists(s_doc[0] + ".png"))
                System.IO.File.Delete(s_doc[0] + ".png");
            if (System.IO.File.Exists(file_name_png))
                System.IO.File.Delete(file_name_png);
        }
    }
}
