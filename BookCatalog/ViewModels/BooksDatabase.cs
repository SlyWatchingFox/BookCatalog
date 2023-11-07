using Npgsql;
using System.Collections.ObjectModel;
using System.Data;

namespace BookCatalog.ViewModels
{
    public class BooksDatabase
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserId { get; set; }
        public string Pass { get; set; }
        public BooksDatabase(string host, int port,string userId, string pass)
        {
            Host = host;
            Port = port;
            UserId = userId;
            Pass = pass;
            CreateDB();
            CreateTable();
        }
        string dbName = "books_collection_db";
        public void SaveTable(ObservableCollection<Book> Books)
        {
            string connectionString = $"Server={Host};Port={Port};Database={dbName}; User Id={UserId};Password={Pass};";
            string npgsql = "SELECT * FROM books";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(npgsql, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                DataTable dt = ds.Tables[0];
                foreach (Book book in Books)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["publisher"] = book.Publisher;
                    newRow["year"] = book.Year;
                    newRow["author"] = book.Author;
                    newRow["name"] = book.Name;
                    dt.Rows.Add(newRow);
                }
                NpgsqlCommand command = new NpgsqlCommand();
                connection.Open();
                command.CommandText = "DELETE FROM books";
                command.Connection = connection;
                command.ExecuteNonQuery();
                NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(adapter);
                adapter.Update(ds);
                ds.Clear();
                adapter.Fill(ds);
            }
        }

        public void ReadTable( ObservableCollection<Book> Books)
        {
            string connectionString = $"Server={Host};Port={Port};Database={dbName}; User Id={UserId};Password={Pass};";
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
                        string publisher = cells[1].ToString();
                        string year = cells[2].ToString();
                        string author = cells[3].ToString();
                        string name = cells[4].ToString();
                        Books.Add(new Book(publisher, year, author, name));
                    }
                }
            }
        }
        public void CreateTable()
        {
            string connectionString = $"Server={Host};Port={Port};Database={dbName}; User Id={UserId};Password={Pass};";
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
                command.CommandText = $"SELECT datname FROM pg_database WHERE datname = '{dbName}'";
                command.Connection = connection;
                if (command.ExecuteScalar().ToString() != dbName)
                {
                    command.CommandText = $"CREATE DATABASE '{dbName}'";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}