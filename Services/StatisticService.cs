using Interfaces;
using Models;

namespace Services
{
    public class StatisticService(IBookService bookService) : IStatisticService
    {
        public int GetTotalBooks()
        {
            return bookService.GetAllBooks().Count;
        }

        public int GetTotalReadersWithBooks()
        {
            return bookService.GetReadersWithBorrowedBooks().Count;
        }
        
        public Dictionary<string, int> GetBooksByGenre()
        {
            return bookService.GetAllBooks()
                .GroupBy(b => b.Genre)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public (Book? oldest, Book? newest) GetOldestAndNewestBooks()
        {
            List<Book?> books = bookService.GetAllBooks();
            if (books.Count == 0)
            {
                return (null, null);
            }

            Book oldest = books.OrderBy(b => b.Year).First();
            Book newest = books.OrderByDescending(b => b.Year).First();
            return (oldest, newest);
        }
    }
}