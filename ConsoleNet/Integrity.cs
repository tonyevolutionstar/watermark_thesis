namespace ConsoleNet
{
    public class Integrity
    {
        public string positions;

        #region positions
        private readonly string top_left;
        private readonly string top_middle;
        private readonly string top_right;
        private readonly string middle_left;
        private readonly string middle;
        private readonly string middle_right;
        private readonly string bottom_left;
        private readonly string bottom_middle;
        private readonly string bottom_right;
        #endregion

        public Integrity(int x_left_barcode, int y_barcode, int x_right_barcode)
        {
            int barcode_width = x_right_barcode - x_left_barcode;
            int right_margin = x_right_barcode + barcode_width / 2;
            int left_margin = x_left_barcode - barcode_width / 2;
            int bottom = y_barcode;

            int m_w = (right_margin + left_margin) / 2;
            int m_h = y_barcode / 3;

            top_left = left_margin.ToString() + "," + left_margin.ToString();
            top_middle = m_w.ToString() + "," + left_margin.ToString();
            top_right = right_margin.ToString() + "," + left_margin.ToString();

            middle_left = left_margin.ToString() + "," + m_h.ToString();
            middle = m_w.ToString() + "," + m_h.ToString();
            middle_right = right_margin.ToString() + "," + m_h.ToString();

            bottom_left = left_margin.ToString() + "," + bottom.ToString();
            bottom_middle = m_w.ToString() + "," + bottom.ToString();
            bottom_right = right_margin.ToString() + "," + bottom.ToString();

            positions = top_left + "|" + top_middle + "|" + top_right + "|"
                + middle_left + "|" + middle + "|" + middle_right + "|"
                + bottom_left + "|" + bottom_middle + "|" + bottom_right;
        }
    }
}
