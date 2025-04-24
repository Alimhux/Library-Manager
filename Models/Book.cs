namespace Models
{
    /// <summary>
    /// Представляет объект книги
    /// </summary>
    public class Book
    {
        private string _title;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Book"/> с пустым названием.
        /// </summary>
        public Book()
        {
            _title = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Book"/> с указанными параметрами.
        /// </summary>
        /// <param name="title">Название книги.</param>
        /// <param name="author">Автор книги.</param>
        /// <param name="genre">Жанр книги.</param>
        /// <param name="year">Год издания книги.</param>
        /// <param name="isbn">ISBN книги.</param>
        public Book(string title, string author, string genre, uint year, string? isbn)
        {
            _title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Isbn = isbn;
        }

        /// <summary>
        /// Получает или задает название книги. Название не может быть пустым или null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Выбрасывается, если значение равно null.</exception>
        public string? Title
        { 
            get => _title;
            set => _title = value ?? throw new ArgumentNullException($"Название книги не может быть пустым или null");
        }

        /// <summary>
        /// Получает или задает автора книги.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Получает или задает жанр книги.
        /// </summary>
        public string? Genre { get; set; }

        /// <summary>
        /// Получает или задает год издания книги.
        /// </summary>
        public uint? Year { get; set; }

        /// <summary>
        /// Получает или задает ISBN книги.
        /// </summary>
        public string? Isbn { get; set; }

        /// <summary>
        /// Возвращает строковое представление объекта книги.
        /// </summary>
        /// <returns>Строка, содержащая информацию о книге.</returns>
        public override string ToString()
        {
            return $"Название: {Title}\n" +
                   $"Автор: {Author}\n" +
                   $"Жанр: {Genre}\n" +
                   $"Год издания: {Year}\n" +
                   $"ISBN: {Isbn ?? "[не указано]"}\n";
        }
    }
}