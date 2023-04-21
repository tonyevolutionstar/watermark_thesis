using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace ConsoleNet
{
    public class SQL_connection
    {
        private int check;
        private string connetionString;
        SqlConnection connection;
        SqlCommand command;
        private string sql;
        SqlDataReader dataReader;
        TrackerServices tracker = new TrackerServices();

        public SQL_connection()
        {
            check = -1;
            connetionString = @"Data Source=localhost;Initial Catalog=Watermark;User ID=antonio;Password=antonio;Integrated Security=true;Trusted_Connection=true";
        }

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
                tracker.WriteFile($"Erro na função check generate id ao verificar se o id do ficheiro já foi gerado - {ex.Message}");
            }
            return check;
        }


        public void Insert_doc(int id, string nome_f, string n_registo, int n_exemplar, int n_copy, string class_seg, string estado_ex, string formato_ex, string utilizador, string data_op, string sigla_principal, string posto_atual, string dominio, string date_time)
        {
            bool insertSuc = false;
            sql = "Use Watermark;INSERT INTO [dbo].[document] (id_document, nome_ficheiro, n_registo, n_exemplar, n_copy, class_seg, estado_ex, formato_ex, utilizador, data_op, sigla_principal, posto_atual, dominio, date_time) VALUES ("
                + id + "," + "'" + nome_f + "'" + "," + "'" + n_registo + "'" + "," + n_exemplar + "," + n_copy + "," + "'" + class_seg + "'" + "," + "'" + estado_ex + "'" + "," + "'" + formato_ex + "'" + "," + "'" + utilizador + "'" + "," + "'" + data_op + "'" + "," + "'" + sigla_principal + "'" + "," + "'" + posto_atual + "'" + "," + "'" + dominio + "'" + ',' + "'" + date_time + "'" + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                int numberRecords = dataReader.RecordsAffected;
                if (numberRecords > 0)
                    insertSuc = true;

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile($"Erro na inserção de valores do documento - {ex.Message}");
            }

            if (insertSuc)
                tracker.WriteFile("inserceção de valores acerca do ficheiro na base de dados " + tracker.finnishState);
            else
                tracker.WriteFile("inserceção de valores acerca do ficheiro na base de dados " + tracker.insertionError);
        }

        public void Insert_watermark(int id_document, int id_barcode, int validation, int x, int y, int x2, int y2, int x_39, int y_39, int x2_39, int y2_39)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[watermark] VALUES ("
            + id_document + "," + id_barcode + "," + validation + "," + x + "," + y + "," + x2 + "," + y2 + "," + x_39 + "," + y_39 + "," + x2_39 + "," + y2_39 + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                int numberRecords = dataReader.RecordsAffected;
                if (numberRecords > 0)
                    tracker.WriteFile("inserceção de valores acerca da marca de água na base de dados " + tracker.finnishState);
                else
                    tracker.WriteFile("inserceção de valores acerca da marca de água na base de dados " + tracker.insertionError);
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile($"inserceção de valores acerca da marca de água na base de dados {tracker.insertionError} com a exceção {ex.Message}");
            }
        }

        public void Insert_barcode(string positions_circleX, string date)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[barcode] VALUES ("
             + "'" + positions_circleX + "'" + "," + "'" + date + "'" + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                int numberRecords = dataReader.RecordsAffected;
                if (numberRecords > 0)
                    tracker.WriteFile("inserceção de valores acerca do código de barra na base de dados " + tracker.finnishState);
                else
                    tracker.WriteFile("inserceção de valores acerca do código de barra na base de dados " + tracker.insertionError);
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile($"inserceção de valores acerca do código de barra na base de dados {tracker.insertionError} com a exceção {ex.Message}");
            }
        }

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
                tracker.WriteFile($"erro ao obter o id do código de barras posições dos caracteres na base de dados com a exceção {ex.Message}");
            }
            return id_barcode;
        }

        public string Search_document(int id_document)
        {
            string result = "";
            sql = "Use Watermark;select nome_ficheiro, utilizador, sigla_principal, posto_atual " +
            " from watermark inner join barcode on watermark.id_barcode = barcode.id_barcode " +
            " inner join document on document.id_document = watermark.id_doc where id_doc = " + id_document;

            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    result = dataReader.GetValue(0).ToString() + ';' + dataReader.GetValue(1).ToString() + ';' + dataReader.GetValue(2).ToString() + ';' + dataReader.GetValue(3).ToString();
                }

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile($"erro ao obter as informações do documento na base de dados com a exceção {ex.Message}");
            }
            return result;
        }

        public void Insert_forense_analises(int id_doc, string line1, string line2, string inter_point, string inter_char, string line1_points, string line2_points)
        {
            bool insertSuc = false;
            sql = "Use Watermark;INSERT INTO [dbo].[forense_analises] VALUES ("
            + id_doc + "," + "'" + line1 + "'" + "," + "'" + line2 + "'" + "," + "'" + inter_point + "'" + "," + "'" + inter_char + "'" + "," + "'" + line1_points + "'" + "," + "'" + line2_points + "'" + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                int numberRecords = dataReader.RecordsAffected;
                if (numberRecords > 0)
                    insertSuc = true;
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile("inserceção de valores acerca da análise forense na base de dados " + tracker.insertionError + " - " + ex.Message);
            }

            if (insertSuc)
                tracker.WriteFile("inserceção de valores acerca da análise forense " + inter_char.Trim() + " na base de dados " + tracker.finnishState);
            else
                tracker.WriteFile("inserceção de valores acerca da análise forense na base de dados " + tracker.insertionError);
        }

        public void Insert_position_char_file(int id_doc, string value_char, int start_x, int start_y, int stop_x, int stop_y)
        {
            int numberRecords = 0;
            sql = "Use Watermark;INSERT INTO [dbo].[position_char_file] VALUES ("
             + id_doc + "," + "'" + value_char + "'" + "," + start_x + "," + start_y + "," + stop_x + "," + stop_y + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                numberRecords = command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile("inserceção de valores acerca das posições dos caracteres no ficheiro na base de dados " + tracker.insertionError + " - " + ex.Message);
            }

            if (numberRecords > 0)
                tracker.WriteFile("inserceção de valores acerca das posições dos caractere " + value_char + " no ficheiro na base de dados " + tracker.finnishState);
            else
                tracker.WriteFile("inserceção de valores acerca das posições dos caractere no ficheiro na base de dados " + tracker.insertionError);
        }

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
                tracker.WriteFile($"erro ao obter os valores da análise forense na base de dados - {ex.Message}");
            }

            var sortedList = returnList.OrderBy(s => float.Parse(s.Split('|')[0].Split(',')[1]))
                           .ThenByDescending(s => float.Parse(s.Split('|')[0].Split(',')[0]))
                           .ToList();

            return sortedList;
        }


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
                tracker.WriteFile($"erro ao obter as posições dos caracteres da base de dados {ex.Message}");
            }
            return returnList;
        }

        public string Get_Positions_Barcode(int id_doc)
        {
            string pos_barcode = "";
            sql = "Use Watermark; Select x, y, x2, y2, x_39, y_39, x2_39, y2_39 from barcode inner join watermark on barcode.id_barcode = watermark.id_barcode where id_doc = " + id_doc + ";";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    string x = dataReader.GetValue(0).ToString();
                    string y = dataReader.GetValue(1).ToString();
                    string x2 = dataReader.GetValue(2).ToString();
                    string y2 = dataReader.GetValue(3).ToString();
                    string x_39 = dataReader.GetValue(4).ToString();
                    string y_39 = dataReader.GetValue(5).ToString();
                    string x2_39 = dataReader.GetValue(6).ToString();
                    string y2_39 = dataReader.GetValue(7).ToString();
                    pos_barcode = $"{x}:{y}:{x2}:{y2}:{x_39}:{y_39}:{x2_39}:{y2_39}";
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile($"erro ao obter as posições do código de barra da base de dados - {ex.Message}");
            }
            return pos_barcode;
        }

        public void Insert_dimensions_doc(int id_doc, int width, int height, int width_bmp, int height_bmp)
        {
            int numberRecords = 0;
            sql = "Use Watermark;INSERT INTO [dbo].[dimensions_document] VALUES ("
            + id_doc + "," + width + "," + height + "," + width_bmp + "," + height_bmp + ");";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                numberRecords = command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile("inserceção de valores acerca das dimensões do ficheiro " + tracker.insertionError + " - " + ex.Message);
            }

            if (numberRecords > 0)
                tracker.WriteFile("inserceção de valores acerca das dimensões do ficheiro " + tracker.finnishState);
            else
                tracker.WriteFile("inserceção de valores acerca das dimensões do ficheiro " + tracker.insertionError);
        }

        public string GetDimensionsDoc_db(int id_doc)
        {
            string dimensions = "";
            sql = "Use Watermark; Select width, height, width_bmp, height_bmp from dimensions_document where id_doc = " + id_doc + ";";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    dimensions = dataReader.GetValue(0).ToString() + ":" + dataReader.GetValue(1).ToString() + ":" + dataReader.GetValue(2).ToString() + ":" + dataReader.GetValue(3).ToString();
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                tracker.WriteFile($"erro ao obter as posições do documento da base de dados - {ex.Message}");
            }
            return dimensions;
        }
    }
}
