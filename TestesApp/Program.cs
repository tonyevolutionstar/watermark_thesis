using System;

namespace TestesApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string errorFile_with_watermark = "O ficheiro que selecionou já foi processado";
            PrincipalMenu principalMenu;

            if (args.Length <= 0 || args.Length > 2)
            {
                Console.WriteLine("Parametros inválidos");
                Console.WriteLine("Argumentos process/retificate Ficheiros\\");
                Console.WriteLine("Exemplo Argumentos - TestesApp.exe retificate Ficheiros\\NACIONAL_1_2022_01000_watermark_4_4_2023_19_39_38.pdf");
            }
            else
            {
                string action = args[0].ToLower();
                string file_choosed = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + args[1];
                Commom commom = new Commom();
                string file_name_without_dir = commom.Get_file_name_without_directory(file_choosed);

                if (action.Equals("process"))
                {
                    if(file_name_without_dir.Contains("watermark"))
                    {
                        Console.WriteLine(errorFile_with_watermark);
                    }
                    else
                    {
                        principalMenu = new PrincipalMenu(file_choosed, action);
                    }
                }
                else if(action.Equals("retificate"))
                {
                    principalMenu = new PrincipalMenu(file_choosed, action);
                }

            }
        }

    }

}
