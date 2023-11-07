using System.Collections.ObjectModel;
using ReactiveUI;

namespace BookCatalog.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();
    private BooksDatabase _booksDb;
    private string _host;
    private int _port;
    private string _userId;
    private string _pass;
    private bool _showConnect = true;
    private bool _showTabl;

    public string Host
    {
        get => _host;
        set => this.RaiseAndSetIfChanged(ref _host, value);
    }
    public int Port
    {
        get => _port;
        set => this.RaiseAndSetIfChanged(ref _port, value);
    }
    public string UserId
    {
        get => _userId;
        set => this.RaiseAndSetIfChanged(ref _userId, value);
    }
    public string Pass
    {
        get => _pass;
        set => this.RaiseAndSetIfChanged(ref _pass, value);
    }
    public bool ShowConnect
    {
        get => _showConnect;
        set => this.RaiseAndSetIfChanged(ref _showConnect, value);
    }
    public bool ShowTabl
    {
        get => _showTabl;
        set => this.RaiseAndSetIfChanged(ref _showTabl, value);
    }
    public void Connect()
    {
        _booksDb = new BooksDatabase("127.0.0.1", 5432, "postgres", "5302");
        ShowConnect = false;
        ShowTabl = true;
        _booksDb.ReadTable(Books);
        if (_host == _booksDb.Host || _port == _booksDb.Port || _userId == _booksDb.UserId|| _pass == _booksDb.Pass)
        {
            ShowConnect = false;
            ShowTabl = true;
            _booksDb.ReadTable(Books);
        }
    }
    public void AddBook()
    {
        Books.Add(new Book("", "", "", ""));
    }
    public void Cancel()
    {
        Books.Clear();
        _booksDb.ReadTable(Books);
    }
    public void Save()
    {
        _booksDb.SaveTable(Books);
    }


}




