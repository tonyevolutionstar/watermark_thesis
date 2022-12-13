using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace WatermarkApp
{
    /// <summary>
    /// Parte visual do programa
    /// </summary>
    public partial class Processamento : Form
    {
        int sizeQrcode = 75; //pixels
        int range = 5;

        Dictionary<Metadata, string> dados;
        List<string> ficheiros = new List<string>();
        Label document_name = new Label();

        private string qr_processed = "_qrcode_processed.png";
        private string qrcode_pdf = "_qrcode.pdf";
        private string file_name;
        private string[] characters;
        int id_doc;
        int id_barcode;


        /// <summary>
        /// Processamento do ficheiro para a originação do documento com watermark
        /// </summary>
        public Processamento(string file_name)
        {
            InitializeComponent();

            dados = PreencherMetadadosParaFicheiros();

            foreach (var kvp in dados)
            {
                ficheiros.Add(kvp.Value);
            }

            document_name.Font = new System.Drawing.Font("Arial", 12);
            document_name.Location = new System.Drawing.Point(600, 9);
            document_name.AutoSize = true;
            document_name.TabIndex = 9;
            this.file_name = file_name;
            Get_PositionChar();
            characters = Read_positionChar_file();
        }

        /// <summary>
        /// Executa o ficheiro jar que vai gerar um ficheiro txt com as posições dos 
        /// caracteres que estão no pdf
        /// </summary>
        private void Get_PositionChar()
        {
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string jarfile = partialPath + @"\Ficheiros\thesis_watermark.jar";
            Process myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardOutput = true;
            myProcess.StartInfo.FileName = "java";
            myProcess.StartInfo.Arguments = "-jar " + '"' + jarfile + '"' + " " + '"' + file_name + '"';
            myProcess.Start();
            myProcess.WaitForExit();
        }

        /// <summary>
        /// Vai ler o ficheiro gerado e colocar os valores numa variavel 
        /// </summary>
        /// <returns></returns>
        private string[] Read_positionChar_file()
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string posFile = s_doc[0] + "_pos.txt";
            string[] lines = System.IO.File.ReadAllLines(posFile, System.Text.Encoding.UTF7);
            
            return lines;
        }

        /// <summary>
        /// Faz load do windows form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Show_file();
        }

        /// <summary>
        /// Gera um id aleatorio que corresponde ao documento que porventura se obtem pela da leitura do qrcode
        /// </summary>
        public int Generate_id()
        {
            Random rd = new Random();
            int rand_num = rd.Next(1, int.MaxValue);
            SQL_connection sql = new SQL_connection();
            if (sql.Check_generate_id(rand_num) == 0)
                return rand_num;
            return 0;
        }


        /// <summary>
        /// Mostra o ficheiro sem o qrcode
        /// </summary>
        private void Show_file()
        {
            id_doc = Generate_id();
 
            string[] show_doc = file_name.Split(new[] { @"Ficheiros\" }, StringSplitOptions.None);
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string[] file = show_doc[1].Split(new[] { ".pdf" }, StringSplitOptions.None);

            document_name.Text = show_doc[1];
            Controls.Add(document_name);
            axAcroPDF1.src = file_name;
            Controls.Add(axAcroPDF1);

            Insert_info_doc(file_name, file[0]);

            if (File.Exists(s_doc[0] + qrcode_pdf))
                File.Delete(s_doc[0] + qrcode_pdf);
        }

        /// <summary>
        /// Mostra ficheiro com o qrcode
        /// </summary>
        private void Process_file()
        {
            string[] show_doc = file_name.Split(new[] { @"Ficheiros\" }, StringSplitOptions.None);
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
     
            if (File.Exists(s_doc[0] + qrcode_pdf))
            {
                MessageBox.Show("Já processei o ficheiro, por favor aceite ou rejeite");  
            }
            else
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                MessageBox.Show("Em processamento");
                document_name.Text = show_doc[1];
                Controls.Add(document_name);
                Generate_qrcode(s_doc[0]);
    
                axAcroPDF1.src = s_doc[0] + qrcode_pdf;
                Controls.Add(axAcroPDF1);
                MessageBox.Show("Documento com watermarking gerado, por favor aceite ou reprove");
                Delete_aux_files(s_doc[0]);
            
                watch.Stop();
                
                Console.WriteLine($"Execution Time {TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)}");
            }
        }


        /// <summary>
        /// Insere o documento na base de dados 
        /// </summary>
        /// <param name="original_file">ficheiro normal</param>
        /// <param name="file_name">nome do ficheiro o caminho todo</param>
        private void Insert_info_doc(string original_file, string file_name)
        {
            SQL_connection sql = new SQL_connection();
            foreach (KeyValuePair<Metadata, string> item in dados)
            {
                if (item.Value.Equals(original_file))
                {
                    sql.Insert_doc(id_doc, file_name, item.Key.NumeroRegisto, item.Key.NumeroExemplar, item.Key.NumeroCopia, item.Key.ClassificacaoSeguranca, item.Key.EstadoExemplar.ToString(), item.Key.FormatoExemplar.ToString(), item.Key.Utilizador, item.Key.DataOperacao.ToString(), item.Key.SiglaPrincipal, item.Key.PostoAtual, item.Key.Dominio);
                }
            }
        }


        /// <summary>
        /// Apaga os ficheiros auxiliares criados
        /// </summary>
        /// <param name="filename">Nome do ficheiro</param>
        private void Delete_aux_files(string filename)
        {
            List<string> delete_files = new List<string>();
            string image = filename + ".png";
            delete_files.Add(image);
            for (int i = 1; i <= 9; i++)
                delete_files.Add(filename + "_qrcode_" + i + ".png");

            string qr_handled = filename + "_qrcode_handled.png";
            delete_files.Add(qr_handled);
            string qr_final = filename + "_qrcode_final.png";
            delete_files.Add(qr_final);

            string processed_pdf = filename + qr_processed;
            delete_files.Add(processed_pdf);
            string processed_png = filename + "_qrcode_processed.png";
            delete_files.Add(processed_png);

            string barcode = filename + "_barcode.png";
            delete_files.Add(barcode);
            
            try
            {
                foreach (string file in delete_files)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
        }


        /// <summary>
        /// Gera um qrcode
        /// </summary>
        /// <param name="file_name">nome do ficheiro sem extensao pdf</param>
        private void Generate_qrcode(string file_name)
        {
            QRcode qrcode = new QRcode(file_name, sizeQrcode, id_doc, range);
            
            if (id_doc != 0)
            {
                Analise_Forense analise = new Analise_Forense(file_name, sizeQrcode);
                for(int i = 0; i < 9; i++)
                    qrcode.Generate_qrcode(i+1);
                
                DateTime date_time_barcode = DateTime.Now;

                SQL_connection sql = new SQL_connection();
                sql.Insert_posicao(analise.positions, date_time_barcode.ToString());
           
                id_barcode = sql.Get_id_barcode(date_time_barcode.ToString());
                qrcode.Generate_barcode(id_barcode);
                qrcode.Add_barcodes_pdf(analise.positions);
              
                AuxFunc auxFunc = new AuxFunc(file_name + ".pdf", sizeQrcode, characters);

                auxFunc.DrawLines(id_doc, sql, analise.positions, file_name + "_qrcode.pdf");
            }
        }

        /// <summary>
        /// Preence os metadados para os ficheiros
        /// </summary>
        /// <returns></returns>
        private static Dictionary<Metadata, string> PreencherMetadadosParaFicheiros()
        {
            Dictionary<Metadata, string> conteudos = new Dictionary<Metadata, string>();
            #region Preencher Metadados Aleatorios

            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string ficheiroRC = Path.Combine(partialPath, @"Ficheiros\NACIONAL_1_2022_01000.pdf");
            string ficheiroGNSSR = Path.Combine(partialPath, @"Ficheiros\NACIONAL_19208_2022_29000.pdf");
            string ficheiroMDNSR = Path.Combine(partialPath, @"Ficheiros\NACIONAL_23_2022_05000.pdf");
            string ficheiroDELNATOSR = Path.Combine(partialPath, @"Ficheiros\NATO_190_2021_13000.pdf");

            Metadata metadataRegistoRC = new Metadata();
            metadataRegistoRC.NumeroRegisto = @"1/2022/01000";
            metadataRegistoRC.NumeroExemplar = 1;
            metadataRegistoRC.NumeroCopia = 0;
            metadataRegistoRC.ClassificacaoSeguranca = "S";
            metadataRegistoRC.EstadoExemplar = Estado.Arquivado;
            metadataRegistoRC.FormatoExemplar = Formato.Eletronico;
            metadataRegistoRC.Utilizador = "João Francisco";
            metadataRegistoRC.DataOperacao = new DateTime(2022, 01, 31, 15, 01, 35);
            metadataRegistoRC.SiglaPrincipal = @"Decreto do Representante da República para os Açores";
            metadataRegistoRC.PostoAtual = "Registo Central";
            metadataRegistoRC.Dominio = "NACIONAL";

            Metadata metadataRegistoGNSSR = new Metadata();
            metadataRegistoGNSSR.NumeroRegisto = @"19208/2022/29000";
            metadataRegistoGNSSR.NumeroExemplar = 1;
            metadataRegistoGNSSR.NumeroCopia = 1;
            metadataRegistoGNSSR.ClassificacaoSeguranca = "NCL";
            metadataRegistoGNSSR.EstadoExemplar = Estado.Ativo;
            metadataRegistoGNSSR.FormatoExemplar = Formato.Eletronico;
            metadataRegistoGNSSR.Utilizador = "Paulo Guimarães";
            metadataRegistoGNSSR.DataOperacao = new DateTime(2022, 02, 22, 18, 54, 01);
            metadataRegistoGNSSR.SiglaPrincipal = @"NEGÓCIOS ESTRANGEIROS";
            metadataRegistoGNSSR.PostoAtual = "Força-Aérea/SR";
            metadataRegistoGNSSR.Dominio = "NACIONAL";

            Metadata metadataRegistoMDNSR = new Metadata();
            metadataRegistoMDNSR.NumeroRegisto = @"23/2022/05000";
            metadataRegistoMDNSR.NumeroExemplar = 2;
            metadataRegistoMDNSR.NumeroCopia = 5;
            metadataRegistoMDNSR.ClassificacaoSeguranca = "C";
            metadataRegistoMDNSR.EstadoExemplar = Estado.EmDestruicao;
            metadataRegistoMDNSR.FormatoExemplar = Formato.Eletronico;
            metadataRegistoMDNSR.Utilizador = "Bruno Fonseca";
            metadataRegistoMDNSR.DataOperacao = new DateTime(2022, 03, 22, 09, 01, 54);
            metadataRegistoMDNSR.SiglaPrincipal = @"ÓRGÃO OFICIAL DA REPÚBLICA DE ANGOLA";
            metadataRegistoMDNSR.PostoAtual = "ANEPC/SR";
            metadataRegistoMDNSR.Dominio = "NACIONAL";

            Metadata metadataRegistoDELNATOSR = new Metadata();
            metadataRegistoDELNATOSR.NumeroRegisto = @"190/2021/13000";
            metadataRegistoDELNATOSR.NumeroExemplar = 6;
            metadataRegistoDELNATOSR.NumeroCopia = 2;
            metadataRegistoDELNATOSR.ClassificacaoSeguranca = "NU";
            metadataRegistoDELNATOSR.EstadoExemplar = Estado.Ativo;
            metadataRegistoDELNATOSR.FormatoExemplar = Formato.Eletronico;
            metadataRegistoDELNATOSR.Utilizador = "José Barrancos";
            metadataRegistoDELNATOSR.DataOperacao = new DateTime(2022, 04, 19, 12, 05, 12);
            metadataRegistoDELNATOSR.SiglaPrincipal = @"Directive on the Public Disclosure of NATO Information";
            metadataRegistoDELNATOSR.PostoAtual = "Ministério dos Negócios estrangeiros S/R";
            metadataRegistoDELNATOSR.Dominio = "NATO";

            conteudos.Add(metadataRegistoRC, ficheiroRC);
            conteudos.Add(metadataRegistoGNSSR, ficheiroGNSSR);
            conteudos.Add(metadataRegistoMDNSR, ficheiroMDNSR);
            conteudos.Add(metadataRegistoDELNATOSR, ficheiroDELNATOSR);
            #endregion

            return conteudos;
        }

        /// <summary>
        /// Função que ouve sempre que o butao aceitar seja clicado para inserir o qrcode na base de dados e passar para o proximo ficheiro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Aceitar_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string[] show_doc = file_name.Split(new[] { @"Ficheiros\" }, StringSplitOptions.None);
                string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
                string file_name_qrcode = s_doc[0] + qrcode_pdf;
                if (File.Exists(file_name_qrcode))
                {
                    SQL_connection sql = new SQL_connection();
                    sql.Insert_watermark(id_doc,id_barcode,1);
                    MessageBox.Show("Documento aceite");
                }
                else
                {
                    MessageBox.Show("Primeiro tens de clicar em processar, para gerar o ficheiro com o qrcode");
                }
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                MessageBox.Show("Último ficheiro na diretoria Ficheiros " + argumentOutOfRangeException);
            }
        }

        /// <summary>
        /// Função que ouve sempre que o butão rejeita seja clicado para inserir o qrcode na base de dados e processar de novo o ficheiro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rejeitar_btn_Click(object sender, EventArgs e)
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string file_name_qrcode = s_doc[0] + qrcode_pdf;

            if (File.Exists(file_name_qrcode))
            {
                SQL_connection sql = new SQL_connection();
                sql.Insert_watermark(id_doc, id_barcode, 0);
                File.Delete(file_name_qrcode);
                Process_file();
            }
            else
            {
                MessageBox.Show("Primeiro tens de clicar em processar, para gerar o ficheiro com o qrcode");
            }
        }

        /// <summary>
        /// Processa o ficheiro para um novo com watermark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gerar_btn_Click(object sender, EventArgs e)
        {
            Process_file();
        }
    }
}
