using iTextSharp.text.pdf;
using System.IO;

namespace WatermarkApp
{
    /// <summary>
    /// Classe para obter as margens do documento
    /// </summary>
    public class File
    {
        private int width;
        private int height;
        private int sizeCircleX;
        private string file_name;
        public string positions;

        #region positions

        private string top_left;
        private string top_middle;
        private string top_right;
        private string middle_left;
        private string middle;
        private string middle_right;
        private string bottom_left;
        private string bottom_middle;
        private string bottom_right;

        #endregion

        /// <summary>
        /// Obtem as margens        
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sizeCircleX"></param>
        public File(string filename, int sizeCircleX)
        {
            this.file_name = filename+".pdf";
            Commom commom = new Commom();
            commom.getDimensionsDocument(filename);
            width = commom.width;
            height = commom.height;

            this.sizeCircleX = sizeCircleX;
            Get_margins();
            positions = top_left + "|" + top_middle + "|" + top_right + "|"
                + middle_left + "|" + middle + "|" + middle_right + "|"
                + bottom_left + "|" + bottom_middle + "|" + bottom_right;
        }

        public void Get_margins()
        {
            int height_top = height - 25;
            int right_margin = width - 70;
            int left_margin = sizeCircleX - 25;
            int m_w = width / 2;
            int m_h = height / 2;

            top_left = left_margin.ToString() + "," + left_margin.ToString();
            top_middle = m_w.ToString() + "," + left_margin.ToString();
            top_right = right_margin.ToString() + "," + left_margin.ToString();

            middle_left = left_margin.ToString() + "," + m_h.ToString();
            middle = m_w.ToString() + "," + m_h.ToString();
            middle_right = right_margin.ToString() + "," + m_h.ToString();

            bottom_left = left_margin.ToString() + "," + height_top.ToString();
            bottom_middle = m_w.ToString() + "," + height_top.ToString();
            bottom_right = right_margin.ToString() + "," + height_top.ToString();
        }
    }
}
