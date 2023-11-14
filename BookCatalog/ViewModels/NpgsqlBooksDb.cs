using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace BookCatalog.ViewModels
{
    public class NpgsqlBooksDb : IBooksDatabase
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserId { get; set; }
        public string Pass { get; set; }
        private string _dbName = "books_collection_db";
        public NpgsqlBooksDb(string host, int port, string userId, string pass)
        {
            Host = host;
            Port = port;
            UserId = userId;
            Pass = pass;
            CreateDB();
            CreateTable();
        }
        public void SaveTable(ObservableCollection<Book> Books, ObservableCollection<Book> DeleteBooks)
        {
            string connectionString = $"Server={Host};Port={Port};Database={_dbName}; User Id={UserId};Password={Pass};";
            string npgsql = "SELECT * FROM books";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(npgsql, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                DataTable dt = ds.Tables[0];
                var rowsСount = dt.Rows.Count;
                for (int i = 0; i < DeleteBooks.Count; i++)
                {
                    for (int j = 0; j < rowsСount; j++)
                    {
                        if (dt.Rows[j].RowState == DataRowState.Deleted) continue;
                        if (DeleteBooks[i].Id.ToString() == dt.Rows[j]["book_id"].ToString())
                        {
                            dt.Rows[j].Delete();
                            rowsСount--;
                        }
                    }
                }
                for (int i = 0; i < Books.Count; i++)
                {
                    for (int j = 0; j < rowsСount; j++)
                    {
                        if (dt.Rows[j].RowState == DataRowState.Deleted) continue;
                        if (Books[i].Id.ToString() == dt.Rows[j]["book_id"].ToString())
                        {
                            dt.Rows[j]["publisher"] = Books[i].Publisher;
                            dt.Rows[j]["year"] = Books[i].Year;
                            dt.Rows[j]["author"] = Books[i].Author;
                            dt.Rows[j]["name"] = Books[i].Name;
                        }
                    }
                    if (Books[i].Id == null)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["publisher"] = Books[i].Publisher;
                        newRow["year"] = Books[i].Year;
                        newRow["author"] = Books[i].Author;
                        newRow["name"] = Books[i].Name;
                        dt.Rows.Add(newRow);
                    }
                }
                NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(adapter);
                adapter.Update(ds);
                ds.Clear();
                adapter.Fill(ds);
            }
        }

        public void ReadTable(ObservableCollection<Book> Books)
        {
            string connectionString = $"Server={Host};Port={Port};Database={_dbName}; User Id={UserId};Password={Pass};";
            string npgsql = "SELECT * FROM books";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(npgsql, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count != 1) return;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var cells = row.ItemArray;
                    if (cells.Length == 5)
                    {
                        int id = Convert.ToInt32(cells[0]);
                        string publisher = cells[1].ToString();
                        string year = cells[2].ToString();
                        string author = cells[3].ToString();
                        string name = cells[4].ToString();
                        Books.Add(new Book(id, publisher, year, author, name));
                    }
                }
            }
        }
        public void CreateTable()
        {
            string connectionString = $"Server={Host};Port={Port};Database={_dbName}; User Id={UserId};Password={Pass};";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS books (" +
                    "book_id serial PRIMARY KEY," +
                    "publisher VARCHAR ( 50 ) NOT NULL," +
                    "year VARCHAR ( 50 ) NOT NULL," +
                    "author VARCHAR ( 50 ) NOT NULL," +
                    "name VARCHAR ( 50 ) NOT NULL);";
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
        }
        public void CreateDB()
        {
            string connectionString = $"Server={Host};Port={Port}; User Id={UserId};Password={Pass};";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.CommandText = $"SELECT datname FROM pg_database WHERE datname = '{_dbName}'";
                command.Connection = connection;
                if (command.ExecuteScalar() == null)
                {
                    command.CommandText = $"CREATE DATABASE {_dbName}";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}