﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
                            string ch = Get_Value_in(bmp, res.X, res.Y);
                            if (!String.IsNullOrEmpty(ch) && !ch.Equals("") && !String.IsNullOrWhiteSpace(ch))
                            {
                                string line1_points = A.X + "," + A.Y + ":" + B.X + "," + B.Y;
                                string line2_points = C.X + "," + C.Y + ":" + D.X + "," + D.Y;
                                string inter_point = res.X + "," + res.Y;

                                if(values_inter.TryGetValue(ch, out List<Point> list))
                                {
                                    list.Add(res);
                                }
                                else
                                {
                                    list = new List<Point> ();
                                    list.Add(res);
                                    values_inter.Add(ch, list);
                                }
                                values.Add(id_doc + "|" + combs[i] + "|" + combs[j] + "|" + inter_point + "|" + ch + "|" + line1_points + "|" + line2_points);
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

                    int threshold = 10; // set the threshold for the absolute difference between the coordinates
                    bool isWithinThreshold = false;
                    for (int j = 1; j <= threshold; j++)
                    {
                        if (Math.Abs(p1.X - p2.X) == j || Math.Abs(p1.Y - p2.Y) == j || Math.Abs(p1.X - p2.X) == j && Math.Abs(p1.Y - p2.Y) == j)
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
                
                int x_circle = int.Parse(circles[0]) * bmp.Width / w ;
                int y_circle = int.Parse(circles[1]) * bmp.Height / h;

                // escolher posição adicional aleatóriamente para dificultar a vida aos hackers
                Random random = new Random();
                int randomX = random.Next(min_random, max_random);
                int randomY = random.Next(min_random, max_random);
                //Console.WriteLine($"Ponto {i+1} com desfazamento no x de {randomX} e no y de {randomY}");
              
                Point circles_l = new Point(x_circle + randomX, y_circle - randomY);
                Point circles_r = new Point(x_circle - randomX, y_circle - randomY); 
                Point circles_b = new Point(x_circle - randomX, y_circle);
                //Point p = new Point(x_circle, y_circle);

                circle_points.Add("point" + (i + 1) + "_l", circles_l);
                circle_points.Add("point" + (i + 1) + "_r", circles_r);
                circle_points.Add("point" + (i + 1) + "_b", circles_b);
                /*
                using(Graphics g = Graphics.FromImage(bmp))
                {
                    Font drawFont = new Font("Arial", 8);
                    SolidBrush drawBrush = new SolidBrush(Color.Blue);
                    g.DrawString("p", drawFont, drawBrush, p);
                    g.DrawString("l", drawFont, drawBrush, circles_l);
                    g.DrawString("r", drawFont, drawBrush, circles_r);
                    g.DrawString("b", drawFont, drawBrush, circles_b);
                }
                bmp.Save("test.png");
                */
            }
            return circle_points;
        }


        private Point Intersection(Point A, Point B, Point C, Point D)
        {
            int x1 = A.X;
            int x2 = B.X;
            int x3 = C.X;
            int x4 = D.X;

            int y1 = A.Y;
            int y2 = B.Y;
            int y3 = C.Y;
            int y4 = D.Y;
            double t = (double) ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
            double u = (double) ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            Point inter = new Point();
            if ((t >= 0 && t <= 1) && (u >= 0 && u <= 1))
            {
                double x = x3 + u * (x4 - x3);
                double y = y3 + u * (y4 - y3);
                inter = new Point((int)x, (int)y);
            }
            return inter;
        }

        /// <summary>
        /// Obtem a letra próxima ao valor da interseção
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Letra</returns>
        private string Get_Value_in(Bitmap bmp, int x, int y)
        {
            string ch = "";
            List<string> pos_char = sql.Get_characters_Pos(id_doc);

            foreach (string values in pos_char)
            {
                string[] val = values.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries); ;
                string[] pos_val = val[1].Split(',');
                int start_x = Convert.ToInt32(int.Parse(pos_val[0]) * bmp.Width / w);
                int start_y = Convert.ToInt32(int.Parse(pos_val[1]) * bmp.Height / h);
                int stop_x = Convert.ToInt32(int.Parse(pos_val[2]) * bmp.Width / w);
                int final_y = Convert.ToInt32(int.Parse(pos_val[3]) * bmp.Height / h);

                // verifica se o valor da interseção está proximo a uma letra
                if (x >= start_x && x <= stop_x && y >= start_y && y <= final_y)
                {
                    return val[0]; 
                }
            }
            return ch;
        }


        /// <summary>
        /// Usado pela Analise Forense
        /// </summary>
        /// <param name="return_list"></param>
        /// <param name="watermark_file"></param>
        /// <param name="difference_x"></param>
        /// <param name="difference_y"></param>
        /// <param name="angle"></param>
      
        public string DrawImage(List<string> return_list, string watermark_file, int difference_x, int difference_y, int angle)
        {
            string f = commom.Convert_pdf_png(watermark_file);
        
            using (Bitmap bmp = new Bitmap(f))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Pen yellow = new Pen(Color.Yellow, 3);
                    int width = 15;
                    int height = 20;
                    int startAngle = 0;
                    int sweepAngle = 360;

                    Font drawFont = new Font("Arial", 8);
                    SolidBrush drawBrush = new SolidBrush(Color.Blue);

                    for (int i = 0; i < return_list.Count; i++)
                    {
                        string[] values = return_list[i].Split('|');
                        string[] inter_point = values[0].Split(',');
                        string ch = values[1];

                        int res_x = Convert.ToInt32(inter_point[0]);
                        int res_y = Convert.ToInt32(inter_point[1]);

                        int new_x = res_x + difference_x * (int)Math.Cos(angle);
                        int new_y = res_y + difference_y * (int)Math.Sin(angle);

                        Point intersection = new Point(new_x, new_y);
                        g.DrawString(ch, drawFont, drawBrush, intersection);
                        g.DrawArc(yellow, intersection.X, intersection.Y, width, height, startAngle, sweepAngle);
                    }

                    filename = f.Split(new[] { ".png" }, StringSplitOptions.None);
                    bmp.Save(filename[0] + integrity_extension + ".png");
                    g.Dispose();
                    bmp.Dispose();
                }
            }
            return filename[0] + integrity_extension + ".png";
        }
    }
}
