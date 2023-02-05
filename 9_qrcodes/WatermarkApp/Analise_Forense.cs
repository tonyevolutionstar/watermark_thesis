using iTextSharp.text.pdf;
using System;
using System.IO;

namespace WatermarkApp
{
    /// <summary>
    /// Classe para proceder a análise forense do ficheiro
    /// </summary>
    public class Analise_Forense
    {
        private int width;
        private int height;
        private int sizeCircleX;
        private string file_name;
        public string positions;

        #region positions
        /// <summary>
        /// 1 qrcode 
        /// </summary>
        public string top_left;
        /// <summary>
        /// 2 qrcode 
        /// </summary>
        public string top_middle;
        /// <summary>
        /// 3 qrcode 
        /// </summary>
        public string top_right;

        /// <summary>
        /// 4 qrcode 
        /// </summary>
        public string middle_left;
        /// <summary>
        /// 5 qrcode 
        /// </summary>
        public string middle;
        /// <summary>
        /// 6 qrcode 
        /// </summary>
        public string middle_right;

        /// <summary>
        /// 7 qrcode 
        /// </summary>
        public string bottom_left;
        /// <summary>
        /// 8 qrcode 
        /// </summary>
        public string bottom_middle;
        /// <summary>
        /// 9 qrcode 
        /// </summary>
        public string bottom_right;
        #endregion

        /// <summary>
        /// Obtem as margins        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sizeCircleX"></param>
        public Analise_Forense(string filename, int sizeCircleX)
        {
            file_name = filename+".pdf";
            using (Stream inputPdfStream = new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true;
                width = (int)reader.GetPageSize(1).Width;
                height = (int)reader.GetPageSize(1).Height;
            }
            this.sizeCircleX = sizeCircleX;
            Get_margins();
            positions = top_left + "|" + top_middle + "|" + top_right + "|"
                  + middle_left + "|" + middle + "|" + middle_right + "|"
                  + bottom_left + "|" + bottom_middle + "|" + bottom_right;
        }

        public void Get_margins()
        {
            int height_top = height - sizeCircleX - 55;
            int r = width - 80; // margem direita
            int l = sizeCircleX + 55; // margem esquerda
            
            double m = width/ 2;
            double m_h = height / 2;

            top_left = l.ToString() + "," + l.ToString();
            top_middle = Math.Round(m).ToString() + "," + l.ToString();
            top_right = r.ToString() + "," + l.ToString(); 

            middle_left = l.ToString() + "," + Math.Round(m_h).ToString();
            middle = Math.Round(m).ToString() + "," + Math.Round(m_h).ToString();
            middle_right = r.ToString() + "," + Math.Round(m_h).ToString();

            bottom_left = l.ToString() + "," + height_top.ToString();
            bottom_right = r.ToString() + "," + height_top.ToString();
            bottom_middle = Math.Round(m).ToString() + "," + height_top.ToString();
        }
    }
}
