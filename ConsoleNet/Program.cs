using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace ConsoleNet
{
    public class Program
    {

        static void Main(string[] args)
        {
            string errorFile_with_watermark = "O ficheiro que selecionou já foi processado";

            if (args.Length <= 0 || args.Length > 2)
            {
                Console.WriteLine("Parametros inválidos");
                Console.WriteLine("Argumentos process/retificate Ficheiros\\");
                Console.WriteLine("Exemplo Argumentos - ConsoleNet.exe retificate \\Ficheiros\\NACIONAL_1_2022_01000_watermark_x_x_x_x_x_x.pdf");
                Console.WriteLine("ConsoleNet.exe process \\Ficheiros\\NACIONAL_1_2022_01000.pdf");
                Console.WriteLine("ConsoleNet.exe convert \\Ficheiros\\NACIONAL_1_2022_01000.pdf");
            }
            else
            {
                string action = args[0].ToLower();
                string file_choosed = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + args[1];
                Commom commom = new Commom();
                string file_name_without_dir = commom.Get_file_name_without_directory(file_choosed);

                if (action.Equals("process"))
                {
                    if (file_name_without_dir.Contains("watermark"))
                    {
                        Console.WriteLine(errorFile_with_watermark);
                    }
                    else
                    {
                       new PrincipalMenu(file_choosed, action);
                    }
                }
                else if (action.Equals("retificate"))
                {
                    new PrincipalMenu(file_choosed, action);
                }
                else if (action.Equals("scale"))
                {
                    ChangeDocument ch_doc = new ChangeDocument(file_choosed);
                    ch_doc.Scale(0.5m);
                    for (decimal i = 0.8m; i < 1m; i += 0.05m)
                    {
                        ch_doc.Scale(i);
                        Console.WriteLine($"scale percentage {i * 100}");
                    }
                }
                else if (action.Equals("rotate"))
                {
                    ChangeDocument ch_doc = new ChangeDocument(file_choosed);
            
                    string img_file = commom.Convert_pdf_png(file_choosed);

                    for (int i = -5; i < 6; i++)
                    {
                        ch_doc.RotateDoc(img_file, i);
                        ch_doc.AddImage_pdf(i);
                    }
                
                    if (File.Exists(img_file))
                        File.Delete(img_file);
                }
            }
        }
    }
}
