using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
namespace BookCatalog.ViewModels
{
    public class MssqlBooksDb : IBooksDatabase
    {
        public string Host { get; set; }
        private string _dbName = "books_collection_db";
        public MssqlBooksDb(string host)
        {
            Host = host;
            if (string.IsNullOrWhiteSpace(Host)) Host = "(localdb)\\mssqllocaldb";
            CreateDB();
            CreateTable();
        }
        public void SaveTable(ObservableCollection<Book> Books, ObservableCollection<Book> DeleteBooks)
        {
            string connectionString = $"Server={Host};Database={_dbName};Trusted_Connection=True;";
            string sql = "SELECT * FROM books";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
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
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(ds);
                ds.Clear();
                adapter.Fill(ds);
            }
        }

        public void ReadTable(ObservableCollection<Book> Books)
        {
            string connectionString = $"Server={Host};Database={_dbName};Trusted_Connection=True;";
            string sql = "SELECT * FROM books";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
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
            string connectionString = $"Server={Host};Database={_dbName};Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT 1 FROM sys.Objects WHERE Object_id = OBJECT_ID('books')";
                command.Connection = connection;
                object s = command.ExecuteScalar();
                if (command.ExecuteScalar() == null)
                {
                    command.CommandText = "CREATE TABLE books (" +
                    "book_id INT PRIMARY KEY IDENTITY," +
                    "publisher NVARCHAR(50) NOT NULL," +
                    "year NVARCHAR(50) NOT NULL," +
                    "author NVARCHAR(50) NOT NULL," +
                    "name NVARCHAR(50) NOT NULL)";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
        }
        public void CreateDB()
        {
            string connectionString = $"Server={Host};Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = $"SELECT name FROM master.dbo.sysdatabases WHERE name = '{_dbName}'";
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
