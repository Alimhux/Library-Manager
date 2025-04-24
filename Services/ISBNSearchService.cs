using Data;
using Interfaces;
using Models;

namespace Services
{
    public class IsbnSearchService(IBookService bookService) : IIsbnSearchService
    {
        public Book? FindBookByIsbn(string isbn)
        {
            return bookService.GetAllBooks().FirstOrDefault(b => b?.Isbn == isbn);
        }

        public List<Book> FindBooksByCountry(string countryName)
        {
            List<Book?> books = bookService.GetAllBooks();
            // Найдём код страны по её названию
            string? countryCode = DataConstants.Countries.FirstOrDefault(c => c.Value == countryName).Key;
            if (countryCode == null)
            {
                return new List<Book>(); // Если страна не найдена, возвращаем пустой список
            }

            return books.Where(b =>
            {
                try
                {
                    Isbn isbnObj = new Isbn(b.Isbn);
                    return isbnObj.CountryCode == countryCode;
                }
                catch
                {
                    return false;
                }
            }).ToList();
        }

        public List<Book> FindBooksByPublisher(string publisherName)
        {
            List<Book?> books = bookService.GetAllBooks();
            // Найдём код издательства по его названию
            string? publisherCode = DataConstants.Publishers.FirstOrDefault(p => p.Value == publisherName).Key;
            if (publisherCode == null)
            {
                return new List<Book>(); // Если издательство не найдено, возвращаем пустой список
            }

            return books.Where(b =>
            {
                try
                {
                    Isbn isbnObj = new Isbn(b.Isbn);
                    return isbnObj.PublisherCode == publisherCode;
                }
                catch
                {
                    return false;
                }
            }).ToList();
        }

        public List<Book?> FindBooksByPublicationNumber(string publicationNumber)
        {
            List<Book?> books = bookService.GetAllBooks();
            return books.Where(b =>
            {
                try
                {
                    Isbn isbnObj = new Isbn(b.Isbn);
                    return isbnObj.GetPublicationNumber() == publicationNumber;
                }
                catch
                {
                    return false;
                }
            }).ToList();
        }
    }
}