using Models;
namespace Interfaces
{
    public interface IIsbnSearchService
    {
        Book? FindBookByIsbn(string isbn);
        List<Book> FindBooksByCountry(string countryCode);
        List<Book> FindBooksByPublisher(string publisherCode);
        List<Book?> FindBooksByPublicationNumber(string publicationNumber);
    }
}