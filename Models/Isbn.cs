using Data;

namespace Models
{
    public class Isbn
    {
        /// <summary>
        /// Получает или задает валидный ISBN.
        /// </summary>
        private string ValidIsbn { get; set; }

        /// <summary>
        /// Получает код страны из ISBN.
        /// </summary>
        public string CountryCode { get; private set; }

        /// <summary>
        /// Получает код издательства из ISBN.
        /// </summary>
        public string PublisherCode { get; private set; }

        /// <summary>
        /// Получает или задает номер публикации из ISBN.
        /// </summary>
        private string PublicationNumber { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Isbn"/> с указанным ISBN.
        /// </summary>
        /// <param name="isbn">ISBN книги. Должен быть длиной 10 или 13 символов.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если длина ISBN не равна 10 или 13 символам.</exception>
        public Isbn(string isbn)
        {
            if (isbn.Length != 13 && isbn.Length != 10)
            {
                throw new ArgumentException("ISBN must be 13 or 10 characters long");
            }
            CountryCode = string.Empty;
            PublisherCode = string.Empty;
            PublicationNumber = string.Empty;
            ValidIsbn = isbn;
            ParseIsbn();
        }

        /// <summary>
        /// Разбирает ISBN на составляющие: код страны, код издательства и номер публикации.
        /// </summary>
        private void ParseIsbn()
        {
            if (ValidIsbn.Length == 10)
            {
                // Упрощённая схема для ISBN-10
                CountryCode = ValidIsbn.Substring(0, 1); 
                PublisherCode = ValidIsbn.Substring(1, 3); 
                PublicationNumber = ValidIsbn.Substring(4, 5); 
            }
            else
            {
                CountryCode = ValidIsbn.Substring(0, 3); 
                PublisherCode = ValidIsbn.Substring(3, 4); 
                PublicationNumber = ValidIsbn.Substring(7, 5); 
            }
        }

        /// <summary>
        /// Возвращает название страны на основе кода страны из ISBN.
        /// </summary>
        /// <returns>Название страны или "Неизвестная страна", если код не найден.</returns>
        public string GetCountry()
        {
            return DataConstants.Countries.GetValueOrDefault(CountryCode, "Неизвестная страна");
        }

        /// <summary>
        /// Возвращает название издательства на основе кода издательства из ISBN.
        /// </summary>
        /// <returns>Название издательства или "Неизвестное издательство", если код не найден.</returns>
        public string GetPublisher()
        {
            return DataConstants.Publishers.GetValueOrDefault(PublisherCode, "Неизвестное издательство");
        }

        /// <summary>
        /// Возвращает номер публикации из ISBN.
        /// </summary>
        /// <returns>Номер публикации.</returns>
        public string GetPublicationNumber()
        {
            return PublicationNumber;
        }
    }
}