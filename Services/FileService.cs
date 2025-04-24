using Interfaces;
using Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Services
{
    public class FileService : IFileService
    {
        public List<Book> LoadBooks(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
            Console.WriteLine("File does not exist");
            return new List<Book>();

        }

        public Dictionary<int, Reader> LoadReaders(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                List<Reader> readersList = JsonSerializer.Deserialize<List<Reader>>(json) ?? new List<Reader>();
                return readersList.ToDictionary(reader => reader.Id);
            }
            return new();
        }

        public void SaveBooks(List<Book?> books, string filePath)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), // Разрешаем все символы Unicode
                WriteIndented = true // Делаем JSON читаемым (с отступами)
            };
            string json = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath, json);
        }

        public void SaveReaders(Dictionary<int, Reader> readers, string filePath)
        {
            List<Reader> readersList = readers.Values.ToList();
            string json = JsonSerializer.Serialize(readersList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}