using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleNet
{
    public class Process
    {
        private Dictionary<Metadata, string> dados;
        private List<string> ficheiros = new List<string>();

        private string file_name;
        private string[] characters;
        private int id_doc;
        private int id_barcode;
        private string date_time;

        private Commom commom = new Commom();
        private string doc_file;
        private readonly string extension_pos = "_pos.txt";

        private readonly string watermark_file = "_watermark_"; // distinguir ficheiros com e sem marca de água
        private readonly string in_proccess = "Em processamento";

        private TrackerServices tracker = new TrackerServices();
        int status = -1; // 0 rejected, 1 accepted;
        private int accept = 1; // testes - 0 rejected, 1 accepted;

        private string img_file;

        public Process(string file_name)
        {
            this.file_name = file_name;
            tracker.WriteFile("---------- Novo Processamento -----------");
            dados = PreencherMetadadosParaFicheiros();
            foreach (var kvp in dados)
            {
                ficheiros.Add(kvp.Value);
            }
            commom.Get_PositionChar(file_name);
            characters = commom.Read_positionChar_file(file_name);
            tracker.WriteFile($"obtenção da posição dos caracteres do ficheiro {file_name} {tracker.finnishState}");

            doc_file = commom.Get_file_name_using_split(file_name);
            id_doc = Generate_id();
            date_time = $"{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}";

            string[] show_doc = file_name.Split(new[] { commom.files_dir }, StringSplitOptions.None);
            string[] file = show_doc[1].Split(new[] { ".pdf" }, StringSplitOptions.None);

            Insert_info_doc_database(file_name, file[0], date_time);
            Insert_info_char_database();
            Process_file();
        }

        private void Process_file()
        {
            Console.WriteLine(in_proccess);
            Generate_watermark(doc_file);
            Delete_aux_files(doc_file);
            if (accept == 1)
                Accept();
            else if (accept == 0)
                Reject();
            Console.WriteLine("Processamento concluido");
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

        private int Generate_id()
        {
            Random rd = new Random();
            int rand_num = rd.Next(1, int.MaxValue);
            SQL_connection sql = new SQL_connection();
            if (sql.Check_generate_id(rand_num) == 0)
                return rand_num;
            return 0;
        }

        private void Generate_watermark(string file_name)
        {
            Watermark watermark = new Watermark(file_name, id_doc);

            if (id_doc != 0)
            {
                DateTime date_time_barcode = DateTime.Now;

                watermark.Generate_barcode(id_barcode);
                watermark.Generate_barcode_39(id_barcode);
                tracker.WriteFile("códigos de barras criados");
                watermark.Add_watermark_pdf(date_time);
                tracker.WriteFile("códigos de barras adicionado ao ficheiro");

                commom.Return_PositionBarcode(file_name + "_" + commom.extension_watermark + "_" + date_time + ".png");

                Integrity analise = new Integrity(commom.x_barcode_pos, commom.y_barcode_pos, commom.x2_barcode_pos);
                tracker.WriteFile("determinação dos pontos para a análise forense " + tracker.finnishState);

                commom.GetDimensionsDocument(file_name);
                string name = commom.Get_file_name_using_split(file_name);
                img_file = name + ".png";
                commom.GetDimensionsImage(img_file);

                SQL_connection sql = new SQL_connection();
                sql.Insert_barcode(analise.positions, date_time_barcode.ToString());
                sql.Insert_dimensions_doc(id_doc, commom.width, commom.height, commom.width_bmp, commom.height_bmp);
                id_barcode = sql.Get_id_barcode(date_time_barcode.ToString());

                AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name + ".pdf");
                auxFunc.CalculateIntersection(analise.positions);
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

            string png_file = filename + watermark_file + date_time + ".png";
            delete_files.Add(png_file);

            string barcode_39 = filename + "_code39.png";
            delete_files.Add(barcode_39);

            try
            {
                foreach (string file in delete_files)
                {
                    if (File.Exists(file))
                    {
                        tracker.WriteFile($"ficheiro auxiliar {file} apagado");
                        File.Delete(file);
                    }
                }
            }
            catch (IOException ioExp)
            {
                tracker.WriteFile($"erro ao apagar os ficheiros auxiliar - {ioExp.Message}");
            }
        }


        private void Accept()
        {
            status = 1;
            SQL_connection sql = new SQL_connection();
            sql.Insert_watermark(id_doc, id_barcode, status, commom.x_barcode_pos, commom.y_barcode_pos, commom.x2_barcode_pos, commom.y2_barcode_pos, commom.x_39, commom.y_39, commom.x2_39, commom.y2_39);
            tracker.WriteFile("documento aceite na base de dados");
            Console.WriteLine("documento aceite na base de dados");
        }

        private void Reject()
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string file_name_watermark = s_doc[0] + watermark_file + date_time + ".pdf";
            status = 0;
            SQL_connection sql = new SQL_connection();
            sql.Insert_watermark(id_doc, id_barcode, status, commom.x_barcode_pos, commom.y_barcode_pos, commom.x2_barcode_pos, commom.y2_barcode_pos, commom.x_39, commom.y_39, commom.x2_39, commom.y2_39);
            tracker.WriteFile("documento rejeitado na base de dados");
            File.Delete(file_name_watermark);
            Process_file();
        }
    }
}
