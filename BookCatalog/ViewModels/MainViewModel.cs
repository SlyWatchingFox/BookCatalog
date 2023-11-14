using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace BookCatalog.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();
    public ObservableCollection<Book> DeleteBooks { get; set; } = new ObservableCollection<Book>();
    public DbType[] DbTypes { get; set; } = new DbType[]
    {
        new DbType("mssql"),
        new DbType("npgsql")
    };
    public DbType DbTypeSelected { get; set; }
    private IBooksDatabase _booksDb;
    private string _host;
    private int _port;
    private string _userId;
    private string _pass;
    private bool _showConnect = true;
    private bool _showTable;
    private Book _bookDelete;
    private string _messError;
    private bool _dialogIsOpen;
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
    public bool ShowTable
    {
        get => _showTable;
        set => this.RaiseAndSetIfChanged(ref _showTable, value);
    }
    public Book BookDelete
    {
        get => _bookDelete;
        set => this.RaiseAndSetIfChanged(ref _bookDelete, value);
    }
    public string MessError
    {
        get => _messError;
        set => this.RaiseAndSetIfChanged(ref _messError, value);
    }
    public bool DialogIsOpen
    {
        get => _dialogIsOpen;
        set => this.RaiseAndSetIfChanged(ref _dialogIsOpen, value);
    }

    public void Connect()
    {
        try
        {
            _booksDb = BooksDbFactory.GetBooksDb(DbTypeSelected.Title, _host, _port, _userId, _pass);
            ShowConnect = false;
            ShowTable = true;
            _booksDb.ReadTable(Books);
        }
        catch (Exception ex)
        {
            MessError = ex.Message;
            DialogIsOpen = true;
        }
    }
    public void AddBook()
    {
        Books.Add(new Book(null, "", "", "", ""));
    }
    public void Cancel()
    {
        try
        {
            Books.Clear();
            _booksDb.ReadTable(Books);
        }
        catch (Exception ex)
        {
            MessError = ex.Message;
            DialogIsOpen = true;
        }
    }
    public void Delete()
    {
        try
        {
            DeleteBooks.Add(BookDelete);
            Books.Remove(BookDelete);
        }
        catch (Exception ex)
        {
            MessError = ex.Message;
            DialogIsOpen = true;
        }
    }
    public void Save()
    {
        try
        {
            _booksDb.SaveTable(Books, DeleteBooks);
            Books.Clear();
            _booksDb.ReadTable(Books);
        }
        catch (Exception ex)
        {
            MessError = ex.Message;
            DialogIsOpen = true;
        }
    }
}