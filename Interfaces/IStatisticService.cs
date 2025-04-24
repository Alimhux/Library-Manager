using Models;
namespace Interfaces
{
    public interface IStatisticService
    {
        int GetTotalBooks();
        int GetTotalReadersWithBooks();
        Dictionary<string, int> GetBooksByGenre();
        (Book? oldest, Book? newest) GetOldestAndNewestBooks();
    }
}