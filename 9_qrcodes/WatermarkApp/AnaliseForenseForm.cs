using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class AnaliseForenseForm : Form
    {
        /// <summary>
        /// Mostrar resultado da análise forense através de um documento criado com as letras que se devia ter em azul
        /// </summary>
        /// <param name="file_name"></param>
        public AnaliseForenseForm(string file_name)
        {
            InitializeComponent();

            axAcroPDF1.src = file_name;
            Controls.Add(axAcroPDF1);
        }
    }
}
