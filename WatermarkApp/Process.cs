using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace WatermarkApp
{
    /// <summary>
    /// Parte visual do processamento
    /// </summary>
    public partial class Process : Form
    {
        Dictionary<Metadata, string> dados;
        List<string> ficheiros = new List<string>();
        Label document_name = new Label();

        private string file_name;
        private string[] characters;
        int id_doc;
        int id_barcode;
        private string date_time;
        private string jar_file = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Ficheiros\thesis_watermark.jar";
        private bool accept_flag = false;

        private string message_error_file_without_watermark = "Primeiro tem de clicar em processar, para gerar o ficheiro com a marca de água";
        private string already_processed = "Já processei o ficheiro, por favor aceite ou rejeite";
        private string in_proccess = "Em processamento";
        private string doc_watermark_generated = "Documento com marca de água gerado, por favor aceite ou reprove";
        
        private string accepted_Doc = "Documento aceite";
        private string already_accepted = "Documento já aceite";
        private string rejected_Doc = "Documento rejeitado";

        private Commom commom = new Commom();
        private string doc_file = "";
        private string extension_pos = "_pos.txt";
      
        private string watermark_file = "_watermark_"; // distinguir ficheiros com e sem marca de água

        private int x_barcode_pos;
        private int y_barcode_pos;
        private int x2_barcode_pos;
        private int y2_barcode_pos;

        private TrackerServices tracker = new TrackerServices();
        int status = -1; // 0 rejected, 1 accepted;
   

        /// <summary>
        /// Processamento do ficheiro para a originação do documento com marca de água
        /// </summary>
        /// <param name="file_name">Nome do ficheiro sem watermark</param>
        public Process(string file_name)
        {
            tracker.WriteFile("---------- Novo Processamento -----------");
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
            tracker.WriteFile($"obtenção da posição dos caracteres do ficheiro {file_name} {tracker.finnishState}");
            doc_file = commom.Get_file_name_using_split(file_name);
        }

        /// <summary>
        /// Insere as posiçoes dos caracteres do ficheiro na base de dados
        /// </summary>
        private void Insert_info_char_database()
        {
            SQL_connection sql = new SQL_connection();
            foreach (string val in characters)
            {
                string[] values = val.Split('|'); 
                string value_char = values[0];
                string[] positions = values[1].Split(',');
                int start_x = int.Parse(positions[0]);
                int start_y = int.Parse(positions[1]);
                int stop_x = int.Parse(positions[2]);
                int stop_y = int.Parse(positions[3]);
                sql.Insert_position_char_file(id_doc, value_char, start_x, start_y, stop_x, stop_y);
            }
        }
       

        /// <summary>
        /// Executa o ficheiro jar que vai gerar um ficheiro txt com as posições dos 
        /// caracteres que estão no pdf
        /// </summary>
        private void Get_PositionChar()
        {
            System.Diagnostics.Process process_file = new System.Diagnostics.Process();
            process_file.StartInfo.UseShellExecute = false;
            process_file.StartInfo.RedirectStandardOutput = true;
            process_file.StartInfo.FileName = "java";
            process_file.StartInfo.Arguments = "-jar " + '"' + jar_file + '"' + " " + '"' + file_name + '"';
            process_file.Start();
            process_file.WaitForExit();
        }

        /// <summary>
        /// Le o ficheiro gerado das posições dos caracteres no documento e colocar os valores numa variavel 
        /// </summary>
        /// <returns>Linhas do documento</returns>
        private string[] Read_positionChar_file()
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);    
            string posFile = s_doc[0] + extension_pos;
            string[] lines = System.IO.File.ReadAllLines(posFile, System.Text.Encoding.UTF7);
            return lines;
        }

    
        private void Process_Load(object sender, EventArgs e)
        {
            Show_file_without_watermark();
        }

        /// <summary>
        /// Gera um id aleatorio que corresponde ao documento que porventura se obtem pela da leitura do barcode
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


        private void Show_file_without_watermark()
        {
            id_doc = Generate_id();
       
            string[] show_doc = file_name.Split(new[] { commom.files_dir }, StringSplitOptions.None);
            string[] file = show_doc[1].Split(new[] { ".pdf" }, StringSplitOptions.None);

            document_name.Text = show_doc[1];
            Controls.Add(document_name);
            axAcroPDF1.src = file_name;
            Controls.Add(axAcroPDF1);
            date_time = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
            tracker.WriteFile("ficheiro mostrado ao utilizador");

            Insert_info_doc_database(file_name, file[0], date_time);
            Insert_info_char_database();
        }

        /// <summary>
        /// Mostra ficheiro com a marca de água
        /// </summary>
        private void Process_file()
        {
            string[] show_doc = file_name.Split(new[] { commom.files_dir }, StringSplitOptions.None);
     
            if (System.IO.File.Exists(doc_file + watermark_file + date_time + ".pdf"))
            {
                MessageBox.Show(already_processed);  
            }
            else
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                MessageBox.Show(in_proccess);
                document_name.Text = show_doc[1];
                Controls.Add(document_name);
                Generate_watermark(doc_file);

                axAcroPDF1.src = doc_file + watermark_file + date_time + ".pdf";
                Controls.Add(axAcroPDF1);
                MessageBox.Show(doc_watermark_generated);
                Delete_aux_files(doc_file);
                watch.Stop();
                Console.WriteLine($"Execution Time {TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)}");
            }
        }


        private void Insert_info_doc_database(string original_file, string file_name, string date_time)
        {
            SQL_connection sql = new SQL_connection();
            foreach (KeyValuePair<Metadata, string> item in dados)
            {
                if (item.Value.Equals(original_file))
                {
                    sql.Insert_doc(id_doc, file_name, item.Key.NumeroRegisto, item.Key.NumeroExemplar, item.Key.NumeroCopia, item.Key.ClassificacaoSeguranca, item.Key.EstadoExemplar.ToString(), item.Key.FormatoExemplar.ToString(), item.Key.Utilizador, item.Key.DataOperacao.ToString(), item.Key.SiglaPrincipal, item.Key.PostoAtual, item.Key.Dominio, date_time);
                }
            }
        }

        private void Delete_aux_files(string filename)
        {
            List<string> delete_files = new List<string>();
            string image = filename + ".png";
            delete_files.Add(image);

            string barcode = filename + "_barcode.png";
            delete_files.Add(barcode);

            string pos = filename + extension_pos;
            delete_files.Add(pos);

            string watermark_png = filename + watermark_file + date_time + ".png";
            delete_files.Add(watermark_png);
            
            try
            {
                foreach (string file in delete_files)
                {
                    if (System.IO.File.Exists(file))
                    {
                        tracker.WriteFile($"ficheiro auxiliar {file} apagado");
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (IOException ioExp)
            {
                tracker.WriteFile($"erro ao apagar os ficheiros auxiliar - {ioExp.Message}");
            }
        }


        private void Generate_watermark(string file_name)
        {
            Watermark watermark = new Watermark(file_name, id_doc);
      
            if (id_doc != 0)
            {
                DateTime date_time_barcode = DateTime.Now;

                watermark.Generate_barcode(id_barcode);
                tracker.WriteFile("código de barras criado");
                watermark.Add_watermark_pdf(date_time);
                tracker.WriteFile("código de barras adicionado ao ficheiro");

                string pos_barcode = commom.Return_PositionBarcode(file_name + watermark_file + date_time + ".pdf");
                tracker.WriteFile("obtenção das posições do código de barras no ficheiro " + tracker.finnishState);
                string[] val_pos_barcode = pos_barcode.Split(':');
                x_barcode_pos = int.Parse(val_pos_barcode[0]);
                y_barcode_pos = int.Parse(val_pos_barcode[1]);
                x2_barcode_pos = int.Parse(val_pos_barcode[2]);
                y2_barcode_pos = int.Parse(val_pos_barcode[3]);

                Integrity analise = new Integrity(x_barcode_pos, y_barcode_pos, x2_barcode_pos);
                tracker.WriteFile("determinação dos pontos para a análise forense " + tracker.finnishState);

                SQL_connection sql = new SQL_connection();
                sql.Insert_barcode(analise.positions, date_time_barcode.ToString());
                id_barcode = sql.Get_id_barcode(date_time_barcode.ToString());

                AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name + ".pdf");
                auxFunc.CalculateIntersection(analise.positions, file_name + watermark_file + date_time + ".pdf");
            }
        }


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


        private void Accept_btn_Click(object sender, EventArgs e)
        {
            if (accept_flag)
                MessageBox.Show(already_accepted);
            else
            {
                try
                {
                    string[] show_doc = file_name.Split(new[] { commom.files_dir }, StringSplitOptions.None);
                    string doc_name = commom.Get_file_name_using_split(file_name);

                    string file_name_watermark = doc_name + watermark_file + date_time + ".pdf";
                    if (System.IO.File.Exists(file_name_watermark))
                    {
                        status = 1;
                        SQL_connection sql = new SQL_connection();
                        sql.Insert_watermark(id_doc, id_barcode, status, x_barcode_pos, y_barcode_pos, x2_barcode_pos, y2_barcode_pos);
                        tracker.WriteFile("documento aceite na base de dados");
                        MessageBox.Show(accepted_Doc);
                        accept_flag = true;
                    }
                    else
                    {
                        MessageBox.Show(message_error_file_without_watermark);
                    }
                }
                catch (ArgumentOutOfRangeException argumentOutOfRangeException)
                {
                    MessageBox.Show("Último ficheiro na diretoria Ficheiros " + argumentOutOfRangeException);
                }
            }

        }

 
        private void Reject_btn_Click(object sender, EventArgs e)
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string file_name_watermark = s_doc[0] + watermark_file + date_time + ".pdf";

            if (accept_flag)
                MessageBox.Show(already_accepted);
            else
            {
                if (System.IO.File.Exists(file_name_watermark))
                {
                    status = 0;
                    SQL_connection sql = new SQL_connection();
                    sql.Insert_watermark(id_doc, id_barcode, status, x_barcode_pos, y_barcode_pos, x2_barcode_pos, y2_barcode_pos);
                    tracker.WriteFile("documento rejeitado na base de dados");
                    System.IO.File.Delete(file_name_watermark);
                    Process_file();
                    MessageBox.Show(rejected_Doc);
                }
                else
                {
                    MessageBox.Show(message_error_file_without_watermark);
                }
            }
        }

        /// <summary>
        /// Processa de novo o ficheiro com marca de água
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Process_btn_Click(object sender, EventArgs e)
        {
            tracker.WriteFile("processamento do ficheiro a iniciar");
            Process_file();
        }


        private void Remove_doc()
        {
            if (status == -1)
            {
                SQL_connection sql = new SQL_connection();
                sql.Remove_Forense(id_doc);
                tracker.WriteFile("eliminado informações da analise forense, já que o formulário foi fechado sem o documento ser aceite ou rejeitado");
                sql.Remove_position_char_file(id_doc);
                tracker.WriteFile("eliminado informações das posições, já que o formulário foi fechado sem o documento ser aceite ou rejeitado");
                sql.Remove_document(id_doc);
                tracker.WriteFile("eliminado informações do documento, já que o formulário foi fechado sem o documento ser aceite ou rejeitado");
            }
        }


        private void Process_FormClosed(object sender, FormClosedEventArgs e)
        {
            Remove_doc();
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string png_file = s_doc[0] + watermark_file + date_time + ".png";
            string file_name_watermark = s_doc[0] + watermark_file + date_time + ".pdf";

            if (File.Exists(file_name_watermark))
                File.Delete(file_name_watermark);

            if (File.Exists(png_file))
                File.Delete(png_file);
        }
    }
}
