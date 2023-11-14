namespace BookCatalog.ViewModels
{
    public static class BooksDbFactory
    {
        public static IBooksDatabase GetBooksDb(string type, string host, int port, string userId, string pass)
        {
            switch (type)
            {
                case "mssql":
                    return new MssqlBooksDb(host);
                case "npgsql":
                    return new NpgsqlBooksDb(host, port, userId, pass);
                default:
                    return null;
            }
        }
    }
}
