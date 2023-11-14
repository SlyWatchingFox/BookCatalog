using System.Collections.ObjectModel;
namespace BookCatalog.ViewModels
{
    public interface IBooksDatabase
    {
        void SaveTable(ObservableCollection<Book> Books, ObservableCollection<Book> DeleteBooks);
        void ReadTable(ObservableCollection<Book> Books);
        void CreateTable();
        void CreateDB();
        public string Host { get; set; }
    }
}
