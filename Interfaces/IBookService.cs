using Models;
namespace Interfaces
{
    public interface IBookService
    {
        void AddBook(Book? book);
        void UpdateBooks(List<Book?> books);
        void RemoveBookByTitle(string? title);
        void RemoveBookByIndex(int index);
        void BorrowBook(int bookIndex, int readerId);
        void ReturnBook(int readerId);
        string?[] GetAllBooksTitles();
        string[] GetAllReadersNames();
        HashSet<string>? GetAllGenres();
        HashSet<string>? GetAllAuthors();
        List<Book?> GetAllBooks();
        List<Book?> FilterBooks(HashSet<string?> genres = null, HashSet<string>? authors = null, uint? yearFrom = null,
            uint? yearTo = null);

        void SaveData(string path);
        List<Book?> SortBooks(int by, bool ascending = true);
        Dictionary<int, Reader> GetAllReaders();
        List<Reader> GetReadersWithBorrowedBooks();

    }
}