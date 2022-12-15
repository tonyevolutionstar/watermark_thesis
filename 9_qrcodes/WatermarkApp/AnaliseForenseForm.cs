using System.Windows.Forms;

namespace WatermarkApp
{
    public partial class AnaliseForenseForm : Form
    {
        public AnaliseForenseForm(string file_name)
        {
            InitializeComponent();

            axAcroPDF1.src = file_name;
            Controls.Add(axAcroPDF1);
        }
    }
}
