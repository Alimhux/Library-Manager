using Interfaces;
using Models;
using Utilities;

namespace Services
{
    /// <summary>
    /// Отвечает за вс
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IFileService _fileService;
        private List<Book?> _books;
        private Dictionary<int, Reader> _readers;

        // Конструктор с инъекцией зависимостей
        public BookService(IFileService fileService)
        {
            _fileService = fileService;
            _books = new List<Book?>();
            _readers = new Dictionary<int, Reader>();
        }

        // Загрузка данных из файлов
        public void LoadBooks(string booksFilePath)
        {
            _books = _fileService.LoadBooks(booksFilePath);
        }

        public void LoadReaders(string readersFilePath)
        {
            Reader.LastReaderId = 1;
            _readers = _fileService.LoadReaders(readersFilePath);
        }


        // Сохранение данных в файлы
        public void SaveData(string booksFilePath)
        {
            _fileService.SaveBooks(_books, booksFilePath);
        }

        public void AddBook(Book? book)
        {
            _books.Add(book);
        }

        public void UpdateBooks(List<Book?> books)
        {
            _books = books;
        }

        public void RemoveBookByIndex(int index)
        {
            try
            {
                Book? book = _books[index];
                _books.Remove(book);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Книга не была удалена. Введённый индекс выходит за пределы списка всех книг.");
            }
        }

        public void RemoveBookByTitle(string? title)
        {
            Book? book = _books.FirstOrDefault(b => b?.Title == title);
            if (book != null)
            {
                _books.Remove(book);
            }
            else
            {
                throw new ArgumentException("Книга не найдена");
            }
        }

        public void BorrowBook(int bookIndex, int readerId)
        {
            Reader reader = _readers[readerId];
            Book? book = _books[bookIndex];

            if (book != null)
            {
                reader.BorrowedBooks.Add(book, DateTime.Now);
            }
        }

        public void ReturnBook(int readerId)
        {
            Reader reader = _readers[readerId];
            string?[] books = reader.BorrowedBooks.Keys.Select(k => k?.Title).ToArray();
            int bookIndex = MenuWorker.DisplayOptionsMenu("Выберите книгу, которую хотите забрать:",
                "Перемещайте курсор стрелочками Вверх/Вниз",
                books);
            reader.BorrowedBooks.Remove(reader.BorrowedBooks.Keys.FirstOrDefault(b => b?.Title == books[bookIndex - 1]));
        }

        public List<Book?> GetAllBooks()
        {
            return _books;
        }

        public string?[] GetAllBooksTitles()
        {
            return _books.Select(b => b?.Title).ToArray();
        }

        public List<Book?> FilterBooks(HashSet<string?> genres = null, HashSet<string>? authors = null,
            uint? yearFrom = null, uint? yearTo = null)
        {
            IEnumerable<Book?> query = _books.AsEnumerable();
            if (genres is { Count: > 0 })
            {
                query = query.Where(b => genres.Contains(b.Genre));
            }

            if (authors is { Count: > 0 })
            {
                query = query.Where(b => authors.Contains(b.Author));
            }

            if (yearFrom > 0)
            {
                query = query.Where(b => b.Year >= yearFrom.Value);
            }

            if (yearTo > 0)
            {
                query = query.Where(b => b.Year <= yearTo.Value);
            }

            return query.ToList();
        }

        public List<Book?> SortBooks(int by, bool ascending = true)
        {
            IEnumerable<Book?> query = _books.AsEnumerable();
            switch (by)
            {
                case 1:
                    query = ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title);
                    break;
                case 2:
                    query = ascending ? query.OrderBy(b => b.Author) : query.OrderByDescending(b => b.Author);
                    break;
                case 3:
                    query = ascending ? query.OrderBy(b => b.Year) : query.OrderByDescending(b => b.Year);
                    break;
            }
            return query.ToList();
        }

        public bool IsLibEmpty => _books.Count == 0;

        public HashSet<string>? GetAllGenres()
        {
            return _books.Select(b => b.Genre).ToHashSet();
        }

        public HashSet<string>? GetAllAuthors()
        {
            return _books.Select(b => b.Author).ToHashSet();
        }

        public bool IsReadersListEmpty => _readers.Count == 0;
        public Dictionary<int, Reader> GetAllReaders()
        {
            return _readers;
        }

        /// <summary>
        /// Возвращает список читателей, которые брали книги.
        /// </summary>
        /// <returns>Список Reader</returns>
        public List<Reader> GetReadersWithBorrowedBooks()
        {
            List<Reader> readersWithBorrowedBooks = [];
            foreach (int readerInd in _readers.Keys)
            {
                Reader reader = _readers[readerInd];
                if (reader.BorrowedBooks.Count > 0)
                {
                    readersWithBorrowedBooks.Add(reader);
                }
            }

            return readersWithBorrowedBooks;
        }

        public string[] GetAllReadersNames()
        {
            return _readers.Values.ToList().Select(r => $"id: {r.Id} | {r.FullName}").ToArray();
        }
    }
}