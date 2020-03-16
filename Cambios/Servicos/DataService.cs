namespace Cambios.Servicos
{
    using Modelos;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class DataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;

        public DataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\rates.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists rates(RateId int, Code varchar(5), TaxRate real, Name varchar(250))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        /// <summary>
        /// Fazer insert dos dados para a tabela na base de dados
        /// </summary>
        /// <param name="Rates">lista com os dados</param>
        public void SaveData(List<Rate> Rates)
        {
            try
            {
                foreach (var rate in Rates)
                {
                    string sql = string.Format($"insert into rates values({rate.RateId}, '{rate.Code}', '{rate.TaxRate}', '{rate.Name}')");

                    command = new SQLiteCommand(sql, connection);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        /// <summary>
        /// Ir buscar os dados que estão na base de dados
        /// </summary>
        /// <returns>lista do tipo rate</returns>
        public List<Rate> GetData()
        {
            List<Rate> Rates = new List<Rate>();

            try
            {
                string sql = "select RateID, Code, TaxRate, Name from rates";

                command = new SQLiteCommand(sql, connection);

                //Lê cada registo
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Rates.Add(new Rate
                    {
                        RateId = (int)reader["RateID"],
                        Code = (string)reader["Code"],
                        TaxRate = (double)reader["TaxRate"],
                        Name = (string)reader["Name"]
                    });
                }

                connection.Close();
                return Rates;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                return null;
            }

        }

        /// <summary>
        /// Apagar dados da tabela na base de dados
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "delete from rates";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }
    }
}
