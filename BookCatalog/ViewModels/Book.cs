namespace BookCatalog.ViewModels
{
    public class Book
    {
        public int? Id { get; set; }
        public string Publisher { get; set; }
        public string Year { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }
        public Book(int? id,string publisher, string year, string author, string name)
        {
            Id = id;
            Publisher = publisher;
            Year = year;
            Author = author;
            Name = name;
        }
    }
}
