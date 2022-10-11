namespace WatermarkApp
{
    /// <summary>
    /// Classe destinada a obter o nome das cores com base no código RGB
    /// </summary>
    public class AuxColor
    {
        private string white = "(255,255,255)";
        private string green = "(0,128,0)";
        private string color = "";
        /// <summary>
        /// Obtem o nome da cor
        /// </summary>
        public string aux_c = "";


        /// <summary>
        /// Obtem as cores com base no RGB e em intervalos
        /// </summary>
        /// <param name="R">Vermelho</param>
        /// <param name="G">Verde</param>
        /// <param name="B">Azul</param>
        public AuxColor(int R, int G, int B)
        {
            color = "(" + R.ToString() + "," + G.ToString() + "," + B.ToString() + ")";
            if (R == G && G == B && B == R)
            {
                if ((R >= 96 && R <= 248) && (G >= 96 && G <= 248) && (B >= 96 && B <= 248))
                    aux_c = "grey";
                else
                {
                    if (color.Equals(white))
                        aux_c = "white";
                    else if (color.Equals(green))
                        aux_c = "green";
                    else if ((R >= 0 && R <= 24) && (G >= 0 && G <= 24) && (B >= 0 && B <= 24))
                        aux_c = "black";
                }
            }
            else
                aux_c = "black";
        }
    }
}
