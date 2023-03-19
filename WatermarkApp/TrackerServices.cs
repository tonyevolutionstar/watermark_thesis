using System;
using System.IO;


namespace WatermarkApp
{
    /// <summary>
    /// Responsavel por fazer tracking da aplicação
    /// </summary>
    public class TrackerServices
    {
        private string logFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Ficheiros\log.txt";
        public string finnishState = "concluída";
        public string errorState = "erro";
        public string insertionError = "falhou";

        public TrackerServices()
        {

        }

        public void WriteFile(string message)
        {
            using(StreamWriter writer = File.AppendText(logFile))
            {
                writer.WriteLine($"{DateTime.Now} - {message}");
            }
 
        }
    }
}
