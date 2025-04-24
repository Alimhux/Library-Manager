namespace Models
{
    /// <summary>
    /// Представляет объект читателя
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Статическое поле для генерации уникального идентификатора читателя.
        /// </summary>
        public static int LastReaderId = 1;

        /// <summary>
        /// Уникальный идентификатор читателя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полное имя читателя.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Контактная информация читателя (например, email).
        /// </summary>
        public string ContactInfo { get; set; }

        /// <summary>
        /// Словарь, содержащий книги, взятые читателем, и даты их взятия.
        /// </summary>
        public Dictionary<Book?, DateTime> BorrowedBooks { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Reader"/> с указанными параметрами.
        /// </summary>
        /// <param name="fullName">Полное имя читателя.</param>
        /// <param name="contactInfo">Контактная информация читателя.</param>
        public Reader(string fullName, string contactInfo)
        {
            Id = LastReaderId++;
            FullName = fullName;
            ContactInfo = contactInfo;
            BorrowedBooks = new Dictionary<Book?, DateTime>();
        }

        /// <summary>
        /// Преобразует список взятых книг в строковое представление.
        /// </summary>
        /// <returns>Строка, содержащая названия книг и даты их взятия.</returns>
        public string BorrowedBooksToStr()
        {
            string books = "";
            foreach (Book? book in BorrowedBooks.Keys)
            {
                string? titlePadded = book.Title?.PadLeft(BorrowedBooks.Max(b => b.Key.Title.Length));
                books += $"{titlePadded} | {BorrowedBooks[book].Date}\n";
            }
            return books;
        }

        /// <summary>
        /// Возвращает строковое представление объекта читателя.
        /// </summary>
        /// <returns>Строка, содержащая информацию о читателе и взятых книгах.</returns>
        public override string ToString()
        {
            return $"Читатель: {FullName}\n" +
                   $"Почта: {ContactInfo}\n" +
                   $"Взятые книги: \n{(BorrowedBooks.Count > 0 ? BorrowedBooksToStr() : "")}";
        }
    }
}