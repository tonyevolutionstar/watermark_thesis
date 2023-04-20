using System.Drawing;
using System.Drawing.Drawing2D;

namespace TestesApp
{
    public static class Rotator
    {
        public static Bitmap Rotate(Bitmap img, double angle)
        {
            var rotated = new Bitmap(img.Width, img.Height);
            using (var gr = Graphics.FromImage(rotated))
            {
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.TranslateTransform(img.Width / 2, img.Height / 2);
                gr.RotateTransform(-(float)angle);
                gr.DrawImage(img, -img.Width / 2, -img.Height / 2, img.Width, img.Height);
            }
            return rotated;
        }
    }
}
