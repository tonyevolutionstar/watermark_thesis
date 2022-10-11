using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace WatermarkApp
{
    /// <summary>
    /// Processa o ficheiro com o qrcode como uma imagem, para permitir a leitura do qrcode no texto
    /// </summary>
    public class ImageProcess
    {
        private string input_img;

        Bitmap img_original;

        /// <summary>
        /// Recebe a imagem do ficheiro com o qrcode para tratar
        /// </summary>
        /// <param name="file_name">Ficheiro com qrcode convertido para png</param>
        /// <returns>nome do ficheiro</returns>
        public ImageProcess(string file_name)
        {
            input_img = file_name;
        }



        /// <summary>
        /// Vai tratar da imagem.
        /// Substitui todas as cores que nao sejam brancas e cinza por pretas.
        /// Altera o qrcode com base nos vizinhos.
        /// Se tiver preto no branco, muda para branco.
        /// Se tiver preto no cinza, muda para cinza.
        /// </summary>
        public void image_replace()
        {
            string[] filename = input_img.Split(new[] { ".png" }, StringSplitOptions.None);
            change_black();

            Bitmap bitmap = new Bitmap(filename[0] + "_handled.png");

            for (int x = 0; x < bitmap.Width ; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color actual_color = bitmap.GetPixel(x, y);
                    AuxColor auxColor = new AuxColor(actual_color.R, actual_color.G, actual_color.B);
                    if(auxColor.aux_c.Equals("black"))
                    {
                        neighbours(bitmap, x, y, "white", System.Drawing.Color.White);
                        neighbours(bitmap, x, y, "grey", System.Drawing.Color.LightGray);
                    }

                    if (auxColor.aux_c.Equals("grey"))
                    {
                        bitmap.SetPixel(x, y, System.Drawing.Color.LightGray);
                    }
                    if (auxColor.aux_c.Equals("white"))
                    {
                        bitmap.SetPixel(x, y, System.Drawing.Color.White);
                    }
                }
            }

            bitmap.Save(filename[0] + "_final.png");
            bitmap.Dispose();

            Bitmap another_p = new Bitmap(filename[0] + "_final.png");
            for (int x = 0; x < another_p.Width; x++)
            {
                for (int y = 0; y < another_p.Height ; y++)
                {
                    System.Drawing.Color actual_color = another_p.GetPixel(x, y);
                    AuxColor auxColor = new AuxColor(actual_color.R, actual_color.G, actual_color.B);
                    if (auxColor.aux_c.Equals("black"))
                        another_p.SetPixel(x, y, System.Drawing.Color.LightGray);
                }
            }
     
            another_p.Save(filename[0] + "_processed.png");
            another_p.Dispose();
        }

        /// <summary>
        /// muda a imagem para preto
        /// </summary>
        public void change_black()
        {
            string[] filename = input_img.Split(new[] { ".png" }, StringSplitOptions.None);
            img_original = new Bitmap(input_img);

            Rectangle rect = new Rectangle(0, 0, img_original.Width, img_original.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                img_original.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                img_original.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * img_original.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            
            for(int i = 0; i < 3; i++)
                for (int counter = i; counter < rgbValues.Length; counter += 3)
                   if(rgbValues[counter] != 0 && rgbValues[counter] != 255  && rgbValues[counter] != 211    )
                        rgbValues[counter] = 0;

            
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            img_original.UnlockBits(bmpData);

            // Draw the modified image.
            img_original.Save(filename[0] + "_handled.png");
            img_original.Dispose();
        }

        /// <summary>
        /// Verifica os pixels vizinhos (Cima, Baixo, Esquerda, Direita) e muda as cores
        /// </summary>
        /// <param name="bitmap">Imagem</param>
        /// <param name="x">Posição x do pixel</param>
        /// <param name="y">Posição x do pixel</param>
        /// <param name="color_pretendida">Cor que se pretende mudar</param>
        /// <param name="color">Cor a mudar</param>
        public void neighbours(Bitmap bitmap, int x, int y, string color_pretendida, System.Drawing.Color color)
        {
            System.Drawing.Color pos_above = bitmap.GetPixel(x, (y + 1));
            AuxColor color_above = new AuxColor(pos_above.R, pos_above.G, pos_above.B);

            System.Drawing.Color pos_below = bitmap.GetPixel(x, (y - 1));
            AuxColor color_below = new AuxColor(pos_below.R, pos_below.G, pos_below.B);

            System.Drawing.Color pos_next = bitmap.GetPixel((x + 1), y);
            AuxColor color_next = new AuxColor(pos_next.R, pos_next.G, pos_next.B);

            System.Drawing.Color pos_prev = bitmap.GetPixel((x - 1), y);
            AuxColor color_prev = new AuxColor(pos_prev.R, pos_prev.G, pos_prev.B);

            if ((color_next.aux_c.Equals(color_pretendida) && color_below.aux_c.Equals(color_pretendida)) ||
                (color_next.aux_c.Equals(color_pretendida) && color_above.aux_c.Equals(color_pretendida)) ||
                (color_next.aux_c.Equals(color_pretendida) && color_below.aux_c.Equals(color_pretendida)) ||
                (color_prev.aux_c.Equals(color_pretendida) && color_below.aux_c.Equals(color_pretendida)) ||
                (color_prev.aux_c.Equals(color_pretendida) && color_above.aux_c.Equals(color_pretendida)) )
            { 
                bitmap.SetPixel(x, y, color);
            }
        }
    }
}
