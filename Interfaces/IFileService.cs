using Models;
namespace Interfaces
{
    public interface IFileService
    {
        List<Book> LoadBooks(string filePath);
        Dictionary<int, Reader> LoadReaders(string filePath);
        void SaveBooks(List<Book?> books, string filePath);
        void SaveReaders(Dictionary<int, Reader> readers, string filePath);
    }
}