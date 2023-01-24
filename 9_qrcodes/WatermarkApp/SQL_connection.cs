using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WatermarkApp
{
    public class SQL_connection
    {
        private int check ;
        private string connetionString;
        SqlConnection connection;
        SqlCommand command;
        private string sql;
        SqlDataReader dataReader;

        /// <summary>
        /// Destinada a fazer operações na base de dados
        /// </summary>
        public SQL_connection()
        {
            check = -1;
            connetionString = null;
            sql = null;
            connetionString = @"Data Source=localhost;Initial Catalog=Watermark;User ID=antonio;Password=antonio;Integrated Security=true;Trusted_Connection=true";
        }

        /// <summary>
        /// Verifica se o id gerado está na base de dados
        /// </summary>
        /// <param name="ID">número de identificação ficheiro</param>
        /// <returns>0 - não tem, >1 se tem</returns>
        public int Check_generate_id(int ID)
        {
            sql = "Use Watermark;Select COUNT(id_document) as id_document from [dbo].[document] where id_document =" + ID.ToString();
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    check = dataReader.GetInt32(0);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return check;

        }

        /// <summary>
        /// Vai retirar as caracteristicas do ficheiro da base de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Dados do ficheiro</returns>
        public string Read_database_doc(string id)
        {
            string result = "";

            sql = "Use Watermark; Select * from [dbo].[document] where id_document =" + id;
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    //12 columns - id_document, n_registo, n_exemplar, n_copy, class_seg, estado_ex, formato_ex, utilizador, data_op, sigla_principal, posto_atual,dominio
                    result = dataReader.GetValue(0) + ";" + dataReader.GetValue(1) + ";" + dataReader.GetValue(2) + ";" + dataReader.GetValue(3) + ";" + dataReader.GetValue(4) + ";" + dataReader.GetValue(5) + ";" + dataReader.GetValue(6) + ";" + dataReader.GetValue(7) + ";" + dataReader.GetValue(8) + ";" + dataReader.GetValue(9) + ";" + dataReader.GetValue(10) + ";" + dataReader.GetValue(11);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public void Insert_doc(int id, string nome_f, string n_registo, int n_exemplar, int n_copy, string class_seg, string estado_ex, string formato_ex, string utilizador, string data_op, string sigla_principal, string posto_atual, string dominio, string date_time)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[document] (id_document, nome_ficheiro, n_registo, n_exemplar, n_copy, class_seg, estado_ex, formato_ex, utilizador, data_op, sigla_principal, posto_atual, dominio, date_time) VALUES ("
                + id + "," + "'" + nome_f + "'" + "," + "'" + n_registo + "'" + "," + n_exemplar + "," + n_copy + "," + "'" + class_seg + "'" + "," + "'" + estado_ex + "'" + "," + "'" + formato_ex + "'" + "," + "'" + utilizador + "'" + "," + "'" + data_op + "'" + "," + "'" + sigla_principal + "'" + "," + "'" + posto_atual + "'" + "," + "'" + dominio + "'" + ',' + "'" + date_time + "'" + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Faz registo de cada vez que um documento com watermark é gerado, guardando se o utilizador aceitou ou rejeitou
        /// </summary>
        /// <param name="id_document"></param>
        /// <param name="id_barcode"></param>
        /// <param name="validation">validação do documento 0-rejeição, 1-aceitação</param>
        public void Insert_watermark(int id_document, int id_barcode, int validation)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[watermark_qrcode] VALUES ("
            + id_document + "," + id_barcode + "," + validation + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Insere o código de barras na base de dados
        /// id_barcode,posicoes_qrcode,date_time
        /// </summary>
        /// <param name="posicoes_qrcode"></param>
        /// <param name="date"></param>
        public void Insert_posicao(string posicoes_qrcode, string date)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[barcode] VALUES ("
             + "'" + posicoes_qrcode + "'" + "," + "'" + date + "'" + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Função para obter o id do código de barras que se inseriu previamente com base no date time
        /// <param name="date_time"></param>
        /// </summary>
        /// <returns>retorna id </returns>
        public int Get_id_barcode(string date_time)
        {
            int id_barcode = 0;
            sql = "Use Watermark;Select id_barcode from barcode where date_time = " + "'" + date_time + "';";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    id_barcode = (int)dataReader.GetValue(0);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return id_barcode;
        }


        /// <summary>
        /// Vai retornar alguns metadados sobre o ficheiro com o id retirado do código de barras para a retificação
        /// </summary>
        /// <param name="id_document"></param>
        /// <returns>nome_ficheiro;utilizador;sigla_principal;posto_atual</returns>
        public string Search_document(int id_document)
        {
            string result = "";
            sql = "Use Watermark;select nome_ficheiro, utilizador, sigla_principal, posto_atual " +
            " from watermark_qrcode inner join barcode on watermark_qrcode.id_barcode = barcode.id_barcode " +
            " inner join document on document.id_document = watermark_qrcode.id_doc where id_doc = " + id_document;

            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    // 4 colunas nome_ficheiro, utilizador, sigla_principal, posto_atual
                    result = dataReader.GetValue(0).ToString() + ';' + dataReader.GetValue(1).ToString() + ';' + dataReader.GetValue(2).ToString() + ';' + dataReader.GetValue(3).ToString();
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }


        public void Insert_forense_analises(int id_doc, string line1, string line2, string inter_point, string inter_char, string line1_points, string line2_points)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[forense_analises] VALUES ("
             + id_doc + "," + "'" + line1 + "'" + "," + "'" + line2 + "'" + "," + "'" + inter_point + "'" + "," + "'" + inter_char + "'" + "," + "'" + line1_points + "'" + "," + "'" + line2_points + "'" + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Insert_position_char_file(int id_doc, string value_char, int start_x, int start_y, int stop_x, int stop_y)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[position_char_file] VALUES ("
             + id_doc + "," + "'" + value_char + "'" + "," + start_x + "," + start_y + "," + stop_x + "," + stop_y + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Obtem os valores da base de dados e compara
        /// </summary>
        /// <param name="id_doc"></param>
        public List<string> Get_Values_Analise_Forense(int id_doc)
        {
            List<string> returnList = new List<string>();

            sql = "Use Watermark; Select inter_point, inter_char, line1_points, line2_points  from forense_analises where id_doc = " + id_doc + ";";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    string inter_point = dataReader.GetValue(0).ToString();
                    string inter_char = dataReader.GetValue(1).ToString();
                    string line1_points = dataReader.GetValue(2).ToString();
                    string line2_points = dataReader.GetValue(3).ToString();
                    string delimitador = "|";
                    returnList.Add(inter_point + delimitador + inter_char + delimitador + line1_points + delimitador + line2_points);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return returnList;
        }

        /// <summary>
        /// Obtem a posicao dos characteres na base de dados
        /// </summary>
        /// <param name="id_doc"></param>
        /// <returns></returns>
        public List<string> Get_characters_Pos(int id_doc)
        {
            List<string> returnList = new List<string>();

            sql = "Use Watermark; Select value_char, start_x, start_y, stop_x, stop_y from position_char_file where id_doc = " + id_doc + ";";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    string value_char = dataReader.GetValue(0).ToString();
                    string start_x = dataReader.GetValue(1).ToString();
                    string start_y = dataReader.GetValue(2).ToString();
                    string stop_x = dataReader.GetValue(3).ToString();
                    string stop_y = dataReader.GetValue(4).ToString();
                    returnList.Add(value_char + "|" + start_x + "," + start_y + "," + stop_x + "," + stop_y);
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return returnList;
        }


        public void RemoveDuplicatesAnaliseForenseLine2(int id_doc)
        {
            sql = "Use Watermark; WITH cte AS (SELECT line2, inter_char, ROW_NUMBER() OVER(PARTITION BY line2, inter_char ORDER BY line2) row_num FROM forense_analises where id_doc = " + id_doc + ")  DELETE FROM cte WHERE row_num > 1;";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public void RemoveDuplicatesAnaliseForenseLine1(int id_doc)
        {
            sql = "Use Watermark;  WITH cte AS (SELECT line1, inter_char, ROW_NUMBER() OVER(PARTITION BY line1, inter_char ORDER BY line1) row_num FROM forense_analises where id_doc = " + id_doc + ")  DELETE FROM cte WHERE row_num > 1;";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
