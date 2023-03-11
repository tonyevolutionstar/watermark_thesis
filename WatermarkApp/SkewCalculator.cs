using System;

namespace WatermarkApp
{
    public static class SkewCalculator
    {
        public static double FindRotateAngle(Compact compact, int stripX1, int stripX2, int maxSkew = 70)
        {
            var shift = 0d;
            var shift1 = FindBestShift(compact, stripX1, stripX2, -maxSkew, maxSkew);
            var shift2 = FindBestShift(compact, stripX1 + 1, stripX2 + 1, -maxSkew, maxSkew);

            if (Math.Abs(shift1 - shift2) < 2)
                shift = (shift1 + shift2) / 2d;
            else
            if (Math.Abs(shift1) < Math.Abs(shift2))
                shift = shift1;
            else
                shift = shift2;

            return Math.Atan2(shift / compact.Scale, (stripX2 - stripX1) * compact.SourceWidth / compact.Width);
        }

        static int FindBestShift(Compact compact, int x1, int x2, int startShift, int endShift)
        {
            var bestCorr = double.MinValue;
            var bestShift = 0;

            for (int shift = startShift; shift <= endShift; shift++)
            {
                var corr = CalcCorr(compact, x1, x2, shift);
                if (corr > bestCorr)
                {
                    bestCorr = corr;
                    bestShift = shift;
                }
            }

            return bestShift;
        }

        private static double CalcCorr(Compact compact, int x1, int x2, int shift)
        {
            var startIndex = shift < 0 ? -shift : 0;
            var endIndex = shift > 0 ? compact.Height - shift - 1 : compact.Height - 1;

            var strip1 = compact.Strips[x1];
            var strip2 = compact.Strips[x2];

            var res = 0;
            for (int i = startIndex; i <= endIndex; i++)
                res += -Math.Abs(strip1[i] - strip2[i + shift]);

            return 1d * strip1.Length * res / (strip1.Length - Math.Abs(shift));
        }
    }
}
