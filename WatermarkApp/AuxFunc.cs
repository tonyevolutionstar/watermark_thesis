using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace WatermarkApp
{
    /// <summary>
    /// Classe auxiliar para operações no ficheiro
    /// </summary>
    public class AuxFunc
    {
        private string[] filename;
        private int id_doc;
        private SQL_connection sql;
        public int h;
        public int w;
        private Dictionary<string, Point> circle_points;
        private List<string> combs;
        private Commom commom;
        private int numberPoints;
        
        private int min_random = 10; 
        private int max_random = 70;
        private string integrity_extension = "_";

        public AuxFunc(int id_doc, SQL_connection sql, string filename)
        {
            this.id_doc = id_doc;
            this.sql = sql;
            commom = new Commom();
            commom.GetDimensionsDocument(filename);
            integrity_extension += commom.extension_integrity;
            numberPoints = commom.number_points;
            w = commom.width;
            h = commom.height;
        }

        /// <summary>
        /// Obtem a letra próxima ao valor da interseção
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Letra</returns>
        private string Get_Value_in(Bitmap bmp, float x, float y)
        {
            string ch = "";
            List<string> pos_char = sql.Get_characters_Pos(id_doc);

            foreach (string values in pos_char)
            {
                string[] val = values.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                string[] pos_val = val[1].Split(',');
                int start_x = Convert.ToInt32(int.Parse(pos_val[0]) * (float)bmp.Width / w);
                int start_y = Convert.ToInt32(int.Parse(pos_val[1]) * (float)bmp.Height / h);
                int stop_x = Convert.ToInt32(int.Parse(pos_val[2]) * (float)bmp.Width / w);
                int final_y = Convert.ToInt32(int.Parse(pos_val[3]) * (float)bmp.Height / h);

                // verifica se o valor da interseção está proximo a uma letra
                if (x > start_x && x < stop_x && y > start_y && y < final_y)
                {
                    return val[0];
                }
            }
            return ch;
        }


        public void CalculateIntersection(string position, string watermark_file)
        {
            string f = commom.Convert_pdf_png(watermark_file);
            Bitmap bmp = new Bitmap(f);

            circle_points = Obtain_points_surround_circle(position, bmp);
       
            string circle_comb;
            combs = new List<string>();
            Dictionary<string, List<Point>> values_inter = new Dictionary<string, List<Point>>();
            List<string> values = new List<string>();

            foreach (KeyValuePair<string, Point> entry in circle_points)
            {
                string[] val0 = entry.Key.Split('_');
                foreach (KeyValuePair<string, Point> entry2 in circle_points)
                {
                    string[] val1 = entry2.Key.Split('_');
                    if (!val0[0].Equals(val1[0]))
                    {
                        circle_comb = entry.Key + ":" + entry2.Key;
                        if (!combs.Contains(entry2.Key + ":" + entry.Key))
                            combs.Add(circle_comb);
                    }
                }
            }

            // get rects without the origin point be the same
            for(int i = 0; i < combs.Count; i ++)
            {
                string[] points_i = combs[i].Split(':');
                string[] side_watermark_i = points_i[0].Split('_');
                for (int j = 0; j < combs.Count; j ++)
                {
                    string[] points_j = combs[j].Split(':');
                    string[] side_watermark_j = points_j[0].Split('_');
                    if (!side_watermark_i[0].Equals(side_watermark_j[0]))
                    {
                        circle_points.TryGetValue(points_i[0], out Point A);
                        circle_points.TryGetValue(points_i[1], out Point B);
                        circle_points.TryGetValue(points_j[0], out Point C);
                        circle_points.TryGetValue(points_j[1], out Point D);
                  
                        Point res = Intersection(A, B, C, D);
                      
                        if ((res.X > 0 && res.X < bmp.Width) && res.Y > 0 && res.Y < bmp.Height && res.X != 0 && res.Y != 0 && (res.X != A.X && res.Y != A.Y) && (res.X != B.X && res.Y != B.Y) && (res.X != C.X && res.Y != C.Y) && (res.X != D.X && res.Y != D.Y))
                        {
                            string v_i = Get_Value_in(bmp, res.X, res.Y);
                            if (!String.IsNullOrEmpty(v_i))
                            {
                                string[] v = v_i.Split('|');
                                string ch = v[0];
                            
                                if (!String.IsNullOrEmpty(ch) && !ch.Equals("") && !String.IsNullOrWhiteSpace(ch))
                                {
                                    string line1_points = A.X + "," + A.Y + ":" + B.X + "," + B.Y;
                                    string line2_points = C.X + "," + C.Y + ":" + D.X + "," + D.Y;
                                    string inter_point = res.X + "," + res.Y;

                                    if (values_inter.TryGetValue(ch, out List<Point> list))
                                    {
                                        list.Add(res);
                                    }
                                    else
                                    {
                                        list = new List<Point>();
                                        list.Add(res);
                                        values_inter.Add(ch, list);
                                    }
                                    values.Add(id_doc + "|" + combs[i] + "|" + combs[j] + "|" + inter_point + "|" + ch + "|" + line1_points + "|" + line2_points);
                                }
                            }
                        }       
                    }
                }
            }

            //tem repetidos, é necessário remover

            // SORT DICTIONARY 
            Dictionary<string, List<Point>> sorted_value_int = values_inter.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.OrderBy(p => p.Y).ThenBy(p => p.X).ToList());
            Dictionary<string, List<Point>> pointCount = sorted_value_int
                .Select(pair => new KeyValuePair<string, List<Point>>(pair.Key, pair.Value.Distinct().ToList()))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (List<Point> pointList in pointCount.Values)
            {
                for (int i = 0; i < pointList.Count - 1; i++)
                {
                    Point p1 = pointList[i];
                    Point p2 = pointList[i + 1];

                    int threshold = 30; // set the threshold for the absolute difference between the coordinates
                    bool isWithinThreshold = false;
                    for (int j = 0; j <= threshold; j++)
                    {
                        if (Math.Abs((int)p1.X - (int)p2.X) == j || Math.Abs((int)p1.Y - (int)p2.Y) == j || Math.Abs((int)p1.X - (int)p2.X) == j && Math.Abs((int)p1.Y - (int)p2.Y) == j)
                        {
                            isWithinThreshold = true;
                            break;
                        }
                    }
                    if (isWithinThreshold)
                    {
                        pointList.RemoveAt(i + 1);
                        i--;
                    }
                }
            }

            //Remove equals
            foreach (KeyValuePair<string, List<Point>> pair in pointCount)
            {
                foreach(Point p in pair.Value)
                {
                    for (int i = 0; i < values.Count; i++)
                    { 
                        string[] val = values[i].Split('|');
                        string combs_i = val[1];
                        string combs_j = val[2];
                        string[] p_i = val[3].Split(',');
                        string ch = val[4];
                        string line1_points = val[5];
                        string line2_points = val[6];

                        Point inter_point = new Point(int.Parse(p_i[0]), int.Parse(p_i[1]));

                        if (p == inter_point)
                        {
                            sql.Insert_forense_analises(id_doc, combs_i, combs_j, val[3], ch, line1_points, line2_points);
                            break;
                        }
                    }
                }
            }
            bmp.Dispose();
        }

        private Dictionary<string, Point> Obtain_points_surround_circle(string position, Bitmap bmp)
        {
            Dictionary<string, Point> circle_points = new Dictionary<string, Point>();
            for (int i = 0; i < numberPoints; i++)
            {
                string[] pos_circles = position.Split('|');
                string[] circles = pos_circles[i].Split(',');
                int x_circle = Convert.ToInt32(int.Parse(circles[0]) * (float) bmp.Width / w) ;
                int y_circle = Convert.ToInt32(int.Parse(circles[1]) * (float) bmp.Height / h);

                // escolher posição adicional aleatóriamente para dificultar a vida aos hackers
                Random random = new Random();
                int randomX = random.Next(min_random, max_random);
                int randomY = random.Next(min_random, max_random);

                Point circles_l = new Point(x_circle + randomX, y_circle - randomY);
                Point circles_r = new Point(x_circle - randomX, y_circle - randomY); 
                Point circles_b = new Point(x_circle - randomX, y_circle);

                circle_points.Add("point" + (i + 1) + "_l", circles_l);
                circle_points.Add("point" + (i + 1) + "_r", circles_r);
                circle_points.Add("point" + (i + 1) + "_b", circles_b);
            }
            return circle_points;
        }


        private Point Intersection(Point A, Point B, Point C, Point D)
        {
            float x1 = A.X;
            float x2 = B.X;
            float x3 = C.X;
            float x4 = D.X;

            float y1 = A.Y;
            float y2 = B.Y;
            float y3 = C.Y;
            float y4 = D.Y;
            float t = (float) ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
            float u = (float) ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            Point inter = new Point();
            if ((t >= 0 && t <= 1) && (u >= 0 && u <= 1))
            {
                int x = Convert.ToInt16(x3 + u * (x4 - x3));
                int y = Convert.ToInt16(y3 + u * (y4 - y3));
                inter = new Point(x, y);
            }
            return inter;
        }

 

        /// <summary>
        /// Usado pela Analise Forense
        /// </summary>
        /// <param name="return_list"></param>
        /// <param name="watermark_file"></param>
        /// <param name="diff_x"></param>
        /// <param name="diff_y"></param>

        public string DrawImage(List<string> return_list, string watermark_file, int diff_x, int diff_y)
        {
            string f = commom.Convert_pdf_png(watermark_file);
            int sizeLetter = 10;
            Point intersection;

            using (Bitmap bmp = new Bitmap(f))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Pen yellow = new Pen(Color.Yellow, 3);
                    int width = 5;
                    int height = 5;
                    int startAngle = 0;
                    int sweepAngle = 360;
                    string[] lines = new string[] {""};
                    Font drawFont = new Font("Arial", sizeLetter);
                    SolidBrush drawBrush = new SolidBrush(Color.Blue);
                    SolidBrush orBrush = new SolidBrush(Color.Red);

                    for (int i = 0; i < return_list.Count; i++)
                    {
                        string[] values = return_list[i].Split('|');
                        string[] inter_point = values[0].Split(',');
                        string ch = values[1];
                        int res_x = Convert.ToInt16(inter_point[0]);
                        int res_y = Convert.ToInt16(inter_point[1]);
             
                        int res_width = res_x * w / bmp.Width;
                        int res_height = res_y * h / bmp.Height;

                        int new_x = res_width + diff_x;
                        int new_y = res_height + diff_y;
                        
                        //ajustar valores ao scan, porque podem variar
                        if (new_x >= 60 && new_x < 180)
                            new_x += 6;
                      
                        else if (new_x >= 180 && new_x < 230)
                            new_x += 3;
                        else if (new_x > 300 && new_x < 430)
                            new_x -= 4;
                        else if (new_x > 430 && new_x <= 450)
                            new_x -= 7;
                        else if (new_x > 450 && new_x <= 550)
                            new_x -= 10;

                        if (new_y >= 60 && new_y < 190)
                            new_y -= 2;
                        else if (new_y >= 190 && new_y < 240)
                            new_y -= 5;
                        else if (new_y >= 240 && new_y <= 270)
                            new_y -= 6;
                        else if (new_y >= 275 && new_y < 290)
                            new_y -= 2;
                        else if (new_y <= 290 && new_y <= 295)
                            new_y -= 7;
                        else if (new_y > 295 && new_y < 420)
                            new_y -= 9;
                        else if (new_y >= 420 && new_y <= 530)
                            new_y -= 15;
                        else if (new_y >= 790 && new_y <= 842)
                            new_y -= 28;

                        if (!watermark_file.Contains("scan"))
                            intersection = new Point(res_x,res_y);
                        else
                            intersection = new Point(new_x * bmp.Width/w, new_y * bmp.Height/h); //adjust point barcode
                        Console.WriteLine($"{ch.Trim()};{res_width}:{res_height};{new_x}:{new_y}");
                    

                        g.DrawString(ch, drawFont, drawBrush, intersection);
                        g.DrawArc(yellow, intersection.X, intersection.Y, width, height, startAngle, sweepAngle);
                    }

                    filename = f.Split(new[] { ".png" }, StringSplitOptions.None);

                    bmp.Save(filename[0] + integrity_extension + ".png");
                    g.Dispose();
                    bmp.Dispose();
                    if(File.Exists(f))
                        File.Delete(f);
                }
            }
            return filename[0] + integrity_extension + ".png";
        }
    }
}
