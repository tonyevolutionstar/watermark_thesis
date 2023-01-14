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
        private int size_qrcode;
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
        /// Obtem as coordenadas dos qrcodes 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="size_qrcode"></param>
        public Analise_Forense(string filename, int size_qrcode)
        {
            file_name = filename+".pdf";
            using (Stream inputPdfStream = new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new PdfReader(inputPdfStream);
                PdfReader.unethicalreading = true;
                width = (int)reader.GetPageSize(1).Width;
                height = (int)reader.GetPageSize(1).Height;
            }
            this.size_qrcode = size_qrcode;
            Get_positions_qrcodes();
            positions = top_left + "|" + top_middle + "|" + top_right + "|"
                  + middle_left + "|" + middle + "|" + middle_right + "|"
                  + bottom_left + "|" + bottom_middle + "|" + bottom_right;
        }

        public void Get_positions_qrcodes()
        {
            int height_top = height - size_qrcode - 15;
            int r = width - size_qrcode - size_qrcode;
            double m = (width - size_qrcode) / 2;
            double m_h = height / 2;

            top_left = size_qrcode.ToString() + "," + size_qrcode.ToString();
            top_middle = Math.Round(m).ToString() + "," + size_qrcode.ToString();
            top_right = r.ToString() + "," + size_qrcode.ToString();

            middle_left = size_qrcode.ToString() + "," + Math.Round(m_h).ToString();
            middle = Math.Round(m).ToString() + "," + Math.Round(m_h).ToString();
            middle_right = r.ToString() + "," + Math.Round(m_h).ToString();

            bottom_left = size_qrcode.ToString() + "," + height_top.ToString();
            bottom_right = r.ToString() + "," + height_top.ToString();
            bottom_middle = Math.Round(m).ToString() + "," + height_top;
        }
    }
}
