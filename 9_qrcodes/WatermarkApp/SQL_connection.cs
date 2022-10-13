﻿using System;
using System.Data.SqlClient;

namespace WatermarkApp
{
    /// <summary>
    /// Base de dados
    /// </summary>
    public class SQL_connection
    {
        private int check;
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

        /// <summary>
        /// Insere o documento na base de dados
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nome_f"></param>
        /// <param name="n_registo"></param>
        /// <param name="n_exemplar"></param>
        /// <param name="n_copy"></param>
        /// <param name="class_seg"></param>
        /// <param name="estado_ex"></param>
        /// <param name="formato_ex"></param>
        /// <param name="utilizador"></param>
        /// <param name="data_op"></param>
        /// <param name="sigla_principal"></param>
        /// <param name="posto_atual"></param>
        /// <param name="dominio"></param>
        public void Insert_doc(int id, string nome_f, string n_registo, int n_exemplar, int n_copy, string class_seg, string estado_ex, string formato_ex, string utilizador, string data_op, string sigla_principal, string posto_atual, string dominio)
        {
            sql = "Use Watermark;INSERT INTO [dbo].[document] (id_document, nome_ficheiro, n_registo, n_exemplar, n_copy, class_seg, estado_ex, formato_ex, utilizador, data_op, sigla_principal, posto_atual, dominio) VALUES ("
                + id + "," + "'" + nome_f + "'" + "," + "'" + n_registo + "'" + "," + n_exemplar + "," + n_copy + "," + "'" + class_seg + "'" + "," + "'" + estado_ex + "'" + "," + "'" + formato_ex + "'" + "," + "'" + utilizador + "'" + "," + "'" + data_op + "'" + "," + "'" + sigla_principal + "'" + "," + "'" + posto_atual + "'" + "," + "'" + dominio + "'" + ");";
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
            + id_document + "," + id_barcode + "," + validation  + ");";
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
             + "'" + posicoes_qrcode + "'" + "," + "'" +  date + "'" + ");";
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
    }
}
