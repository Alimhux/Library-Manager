using Data;
using Interfaces;
using Models;

namespace Utilities
{
    public class ConsoleHandler
    {
        /// <summary>
        /// Запрашивает данные о новой книге у пользователя и добавляет её в библиотеку.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        /// <param name="validator">Валидатор для проверки корректности ISBN.</param>
        public static void AddBook(IBookService bookService, IsbnValidator validator)
        {
            Console.WriteLine("\n=== Добавление книги ===\n");
            string title = GetStrFromUser("Введите название книги: ",
                "Название не может быть пустым. Повторите ввод:   ");
            string author = GetStrFromUser("Введите имя автора книги: ",
                "Имя автора не может быть пустым. Повторите ввод:  ");
            int genre = MenuWorker.DisplayOptionsMenu("Выберите жанр: ",
                "Перемещайте курсор стрелочками Вниз/Вверх",
                DataConstants.Genres);
            uint year = GetPublicationYear();

            string? isbn = GetIsbnFromUser(validator, "Введите ISBN: ",
                "Некорректный ISBN. Повторите ввод или нажмите Enter, чтобы добавить книгу без ISBN");
            Book book = new Book(title, author, DataConstants.Genres[genre - 1], year, isbn);
            bookService.AddBook(book);
            Console.WriteLine("\u2713 Книга успешно добавлена!");

            AwaitExit();
        }

        /// <summary>
        /// Удаляет книгу из библиотеки по выбору пользователя.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        public static void RemoveBook(IBookService bookService)
        {
            Console.WriteLine("\n=== Удаление книги ===\n");
            int choice = MenuWorker.DisplayOptionsMenu("Выберите способ удаления книги: ",
                "Перемещайте курсор стрелочками Вверх/Вниз",
                "По названию книги",
                "Выбрать из списка доступных книг");
            switch (choice)
            {
                case 1:
                    DelBookByTitle(bookService);
                    break;
                case 2:
                    DelBookByChoice(bookService);
                    break;
            }
        }

        /// <summary>
        /// Удаляет книгу по её названию.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        private static void DelBookByTitle(IBookService bookService)
        {
            try
            {
                Console.Write("Введите название книги:    ");
                string? title = Console.ReadLine();
                bookService.RemoveBookByTitle(title);
                AwaitExit("\n\u2713 Книга успешно удалена!\n\nНажмите Enter, чтобы продолжить.");
            }
            catch (ArgumentException)
            {
                AwaitExit("\nТакой книги нет в наличии.\n\nНажмите Enter, чтобы продолжить.");
            }
        }

        /// <summary>
        /// Удаляет книгу, выбранную пользователем из списка.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        private static void DelBookByChoice(IBookService bookService)
        {
            int choice = MenuWorker.DisplayOptionsMenu("Выберите книгу для удаления",
                "Перемещайте курсор стрелочками Вверх/Вниз",
                bookService.GetAllBooksTitles());
            bookService.RemoveBookByIndex(choice - 1);
            AwaitExit("\n\u2713 Книга успешно удалена!\n\nНажмите Enter, чтобы продолжить.");
        }

        /// <summary>
        /// Выдаёт книгу читателю.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        public static void BorrowBook(IBookService bookService)
        {
            Console.WriteLine("\n=== Выдача книги читателю ===\n");
            int book = MenuWorker.DisplayOptionsMenu("Выберите книгу:",
                "Перемещайте курсор стрелочками Вверх/Вниз",
                bookService.GetAllBooksTitles());
            MenuWorker.DrawMenuFrame("Выберите читателя, который получит книгу",
                string.Empty,
                false,
                bookService.GetAllReadersNames());

            Dictionary<int, Reader> readers = bookService.GetAllReaders();
            int readerId = GetReaderId(readers.Keys.ToArray());
            bookService.BorrowBook(book - 1, readerId);
            readers = bookService.GetAllReaders();
            Console.Clear();
            AwaitExit($"\n\u2713 Книга успешно выдана читателю {readers[readerId].FullName}!\n" +
                      $"\nОбновлённая информация:" +
                      $" \n{DataConstants.UnderLine}\n{readers[readerId]}{DataConstants.UnderLine}\n" +
                      $"\nНажмите Enter, чтобы продолжить.");
        }

        /// <summary>
        /// Возвращает книгу в библиотеку.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        public static void ReturnBook(IBookService bookService)
        {
            if (bookService.GetReadersWithBorrowedBooks().Count == 0)
            {
                Console.WriteLine("Видимо, ещё ни один читатель не взял книгу :(\n" +
                                  "(Вернитесь в главное меню и выдайте кому-нибудь книгу)");
            }
            else
            {
                List<Reader> readers = bookService.GetReadersWithBorrowedBooks();
                DisplayReadersAndBooks(readers);
                int readerId = GetReaderId(readers.Select(r => r.Id).ToArray());
                Console.Clear();
                try
                {
                    bookService.ReturnBook(readerId);
                    Console.WriteLine("Изменения успешно применены! Информация обновлена: ");
                    Console.WriteLine(
                        $" \n{DataConstants.UnderLine}\n{bookService.GetAllReaders()[readerId]}{DataConstants.UnderLine}\n");
                }
                catch (Exception)
                {
                    Console.WriteLine("Не удалось удалить книгу, повторите попытку позже.");
                }
            }

            AwaitExit();
        }

        /// <summary>
        /// Фильтрует книги по заданным критериям.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        public static void FilterBooks(IBookService bookService)
        {
            Console.WriteLine("\n=== Фильтрация книг ===");
            List<string> allGenres = (bookService.GetAllGenres() ?? []).ToList();
            List<string> allAuthors = (bookService.GetAllAuthors() ?? []).ToList();
            HashSet<string?> filteredGenres = new();
            HashSet<string> filteredAuthors = new();
            uint yearFrom = 0, yearTo = 0;
            List<Book> filteredBooks;
            bool run = true;
            bool toDel = false;
            while (run)
            {
                Console.Clear();
                int by = MenuWorker.DisplayOptionsMenu(
                    $"{(toDel ? "Выберите пункт ДЛЯ УДАЛЕНИЯ (!!)" : "Выберите направление фильтации:")}",
                    "Перемещайте курсор стрелочками Вверх/Вниз",
                    $"{(filteredGenres.Count == 0 || toDel ? "По жанру" : "Добавить жанр")}",
                    $"{(filteredAuthors.Count == 0 || toDel ? "По автору" : "Добавить автора")}",
                    $"{(yearFrom == 0 || toDel ? "По году публикации" : "Изменить год публикации")}",
                    "Применить фильтрацию");

                switch (by)
                {
                    case 1:
                        if (toDel)
                        {
                            filteredGenres = new();
                            toDel = false;
                            Console.WriteLine("Пункт фильтрации успешно удалён!");
                            AwaitExit();
                            break;
                        }

                        if (allGenres.Count == 0)
                        {
                            Console.WriteLine("Больше нет жанров для выбора :(");
                            AwaitExit();
                            break;
                        }

                        int genre = MenuWorker.DisplayOptionsMenu("Выберите один из доступных жанров: ",
                            "Перемещайте курсор стрелочками Вниз/Вверх", allGenres.ToArray());
                        string newGenre = allGenres[genre - 1];
                        filteredGenres.Add(newGenre);
                        allGenres.Remove(newGenre);
                        break;
                    case 2:
                        if (toDel)
                        {
                            filteredAuthors = new();
                            toDel = false;
                            Console.WriteLine("Пункт фильтрации успешно удалён!");
                            AwaitExit();
                            break;
                        }

                        if (allAuthors.Count == 0)
                        {
                            Console.WriteLine("Больше нет авторов для выбора :(");
                            AwaitExit();
                            break;
                        }

                        int author = MenuWorker.DisplayOptionsMenu("Выберите одного из доступных авторов    : ",
                            "Перемещайте курсор стрелочками Вниз/Вверх", allAuthors.ToArray());
                        string newAuthor = allAuthors[author - 1];
                        filteredAuthors.Add(newAuthor);
                        allAuthors.Remove(newAuthor);
                        break;
                    case 3:
                        if (toDel)
                        {
                            yearFrom = yearTo = 0;
                            toDel = false;
                            Console.WriteLine("Пункт фильтрации успешно удалён!");
                            AwaitExit();
                            break;
                        }

                        yearFrom = GetPublicationYear("Введите минимальный год выпуска:  ",
                            "Некорректный год, повторите попытку:  ");
                        yearTo = GetPublicationYear("Введите максимальный год выпуска:  ",
                            "Некорректный год, повторите попытку:  ");
                        break;
                    case 4:
                        toDel = false;
                        Console.WriteLine("Жанры для фильтрации: [" + string.Join(", ", filteredGenres) + "]");
                        Console.WriteLine("Авторы для фильтрации: [" + string.Join(", ", filteredAuthors) + "]");
                        Console.WriteLine($"Год публикации для фильтрации: " +
                                          $"{(yearFrom == 0 ? "Не указано" : $"{yearFrom} - {yearTo}")}");
                        AwaitExit("\nНажмите Enter чтобы применить фильтрацию");
                        filteredBooks = bookService.FilterBooks(filteredGenres, filteredAuthors, yearFrom, yearTo);
                        if (filteredBooks.Count == 0)
                        {
                            if (DisplayNooBook() == ConsoleKey.Enter)
                            {
                                run = false;
                                break;
                            }

                            toDel = true;
                        }
                        else
                        {
                            Console.WriteLine("\u2713 Фильтрация проведена успешно! Вот выборка книг: \n");
                            DisplayBooks(filteredBooks);
                            run = false;
                            if (ToSave())
                            {
                                bookService.UpdateBooks(filteredBooks);
                                Console.Clear();
                                Console.WriteLine(
                                    "\n\u2713 Данные обновлены и будут сохранены после выхода из программы!");
                            }

                            AwaitExit();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Сообщает пользователю, что ни одна книга не попала в выборку.
        /// </summary>
        /// <returns>Клавиша, нажатая пользователем.</returns>
        private static ConsoleKey DisplayNooBook()
        {
            Console.WriteLine("К сожалению, ни одна книга не удовлетворяет выбранным фильтрам.");
            Console.WriteLine("Нажмите Enter чтобы выйти в главное меню");
            Console.WriteLine("ИЛИ любую другую клавишу, чтобы изменить выборку ");
            return Console.ReadKey().Key;
        }

        /// <summary>
        /// Отображает список книг.
        /// </summary>
        /// <param name="books">Список книг.</param>
        private static void DisplayBooks(List<Book> books)
        {
            Console.WriteLine(DataConstants.UnderLine + "\n");
            foreach (Book? book in books)
            {
                Console.WriteLine(book);
                Console.WriteLine(DataConstants.UnderLine + "\n");
            }
        }

        /// <summary>
        /// Запрашивает у пользователя, сохранять ли данные после сортировки или фильтрации.
        /// </summary>
        /// <returns>True, если пользователь хочет сохранить данные, иначе False.</returns>
        private static bool ToSave()
        {
            Console.WriteLine("Сохранить данные в таком виде? ");
            Console.WriteLine("\nНажмите Enter, если хотите сохранить их, при выходе из программы");
            Console.WriteLine("ИНАЧЕ - нажмите любую другую клавишу");
            return Console.ReadKey().Key == ConsoleKey.Enter;
        }

        /// <summary>
        /// Сортирует книги по выбранному критерию.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        public static void SortBooks(IBookService bookService)
        {
            Console.WriteLine("\n=== Сортировка книг ===");
            int by = MenuWorker.DisplayOptionsMenu($"Выберите направление сортировки:",
                "Перемещайте курсор стрелочками Вверх/Вниз",
                "По названию", "По автору", "По году публикации");
            bool ascending = 1 == MenuWorker.DisplayOptionsMenu($"Выберите порядок сортировки:",
                "Перемещайте курсор стрелочками Вверх/Вниз",
                $"{(by == 3 ? "По возрастанию" : "По алфавиту (А - Я)")}",
                $"{(by == 3 ? "По убыванию" : "По алфавиту (Я - А)")}");

            List<Book?> sortedBooks = bookService.SortBooks(by, ascending);

            if (sortedBooks.Count > 0)
            {
                Console.WriteLine("\u2713 Сортировка проведена успешно! Вот список книг в отсортированном виде: \n");
                DisplayBooks(sortedBooks);
                if (ToSave())
                {
                    bookService.UpdateBooks(sortedBooks);
                    Console.Clear();
                    Console.WriteLine("\n\u2713 Данные обновлены и будут сохранены после выхода из программы!");
                }
            }
            else
            {
                Console.WriteLine("К сожалению, список книг пуст :(" +
                                  "\nПопробуйте выйти в главное меню и применить сортировку снова");
            }

            AwaitExit();
        }

        /// <summary>
        /// Отображает общую статистику по библиотеке.
        /// </summary>
        /// <param name="bookService">Сервис для работы с книгами.</param>
        /// <param name="statisticsService">Сервис для работы со статистикой.</param>
        public static void GetStatistics(IBookService bookService, IStatisticService statisticsService)
        {
            Console.WriteLine("\n=== Общая статистика ===\n");
            Console.WriteLine($"Количество книг: {statisticsService.GetTotalBooks()}");
            Console.WriteLine(DataConstants.UnderLine);
            Console.WriteLine($"Количество читателей с книгами: {statisticsService.GetTotalReadersWithBooks()}");
            Console.WriteLine(DataConstants.UnderLine);
            if (bookService.GetAllBooks().Count > 0)
            {
                Console.WriteLine($"Количество книг по жанрам:");
                foreach (KeyValuePair<string, int> pair in statisticsService.GetBooksByGenre())
                {
                    Console.WriteLine($"- {pair.Key}: {pair.Value}");
                }

                Console.WriteLine(DataConstants.UnderLine);
            }

            (Book? oldest, Book? newest) = statisticsService.GetOldestAndNewestBooks();
            if (!(oldest is null || newest is null))
            {
                Console.WriteLine($"Самая старая книга: {oldest.Title} ({oldest.Year})");
                Console.WriteLine($"Самая новая книга: {newest.Title} ({newest.Year})");
                Console.WriteLine(DataConstants.UnderLine);
            }

            AwaitExit();
        }

        /// <summary>
        /// Выполняет поиск книги по ISBN
        /// </summary>
        /// <param name="bookService"></param>
        /// <param name="isbnSearchService"></param>
        /// <param name="validator"></param>
        public static void FindBookByIsbn(IBookService bookService, IIsbnSearchService isbnSearchService,
            IsbnValidator validator)
        {
            Console.WriteLine("\n=== Поиск книг по ISBN ===\n");
            int choice = MenuWorker.DisplayOptionsMenu("Как вы хотите осуществить поиск?",
                "Перемещайте курсор стрелочками Вверх/Вниз",
                "По стране", "По коду издательства", "По уникальному номер издания");

            try
            {
                switch (choice)
                {
                    case 1:
                        int country = MenuWorker.DisplayOptionsMenu("Выберите одну из доступных стран",
                            "Перемещайте курсор стрелочками Вверх/Вниз", DataConstants.Countries.Values.ToArray());
                        List<Book> countryBooks =
                            isbnSearchService.FindBooksByCountry(DataConstants.Countries.Values.ToArray()[country - 1]);
                        if (countryBooks.Any())
                        {
                            Console.WriteLine("Вот книги, выпущенные в этой стране! ");
                            DisplayBooks(countryBooks);
                        }
                        else
                        {
                            Console.WriteLine("К сожалению, книги этой страны не найдены.");
                        }

                        AwaitExit();
                        break;

                    case 2:
                        int publisher = MenuWorker.DisplayOptionsMenu("Выберите одну из доступных стран",
                            "Перемещайте курсор стрелочками Вверх/Вниз", DataConstants.Publishers.Values.ToArray());
                        List<Book> publisherBooks =
                            isbnSearchService.FindBooksByPublisher(
                                DataConstants.Publishers.Values.ToArray()[publisher - 1]);
                        if (publisherBooks.Any())
                        {
                            Console.WriteLine("Вот книги, выпущенные под этим издательством! ");
                            DisplayBooks(publisherBooks);
                        }
                        else
                        {
                            Console.WriteLine("К сожалению, книги этого издательства не найдены.");
                        }

                        AwaitExit();
                        break;

                    case 3:
                        while (true)
                        {
                            Console.Clear();
                            string publicationNumber = GetStrFromUser("Введите уникальный номер издания: ",
                                "Некорректный номер издания, повторите ввод");
                            List<Book?> publicationBooks =
                                isbnSearchService.FindBooksByPublicationNumber(publicationNumber);
                            if (publicationBooks.Any())
                            {
                                Console.WriteLine("Вот книги c этим номером издания! ");
                                DisplayBooks(publicationBooks);
                                AwaitExit();
                                break;
                            }

                            Console.WriteLine("\nКниги не найдены. Нажмите Enter, для выхода в главное меню");
                            Console.WriteLine("ИЛИ любую другую клавишу, чтобы повторить ввод");
                            if (Console.ReadKey().Key == ConsoleKey.Enter)
                            {
                                break;
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                AwaitExit();
            }
        }
    
    
        /// <summary>
        /// Завершает работу, сохраняет файл
        /// </summary>
        /// <param name="bookService"></param>
        /// <param name="path"></param>
        /// <param name="whereToSave"></param>
        /// <returns></returns>
        public static bool FinishWork(IBookService bookService, string path, int whereToSave)
        {
            try
            {
                if (path == "books.json")
                {
                    bookService.SaveData($"../../../{path}");
                    AwaitExit($"Файл {path} перезаписан и находится в папке проекта" +
                              "\nНажмите Enter, чтобы выйти из программы.");
                }
                else if (whereToSave == 1)
                {
                    bookService.SaveData(path);
                    AwaitExit(
                        $"Файл {Path.GetFileName(path)} успешно перезаписан и находится в указанной вами директории." +
                        "\nНажмите Enter, чтобы выйти из программы.\n");
                }
                else
                {
                    bookService.SaveData($"../../../{path}");
                    AwaitExit($"Файл {Path.GetFileName(path)} успешно создан и находится в папке проекта" +
                              "\nНажмите Enter, чтобы выйти из программы.\n");
                }

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Не удалось сохранить данные в файл.");
                return false;
            }
        }

        /// <summary>
        /// Получает (путь к файлу / имя файла) от пользователя
        /// </summary>
        /// <param name="toRead">Является ли файл входным или выходным</param>
        /// <param name="books">Книги или читателей нужно загрузить</param>
        /// <returns>Имя файла/Путь к файлу, в зависимости </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <summary />
        public string GetFileNameOrFilePathFromUser(bool toRead = true, bool books = true)
        {
            Console.Write($"Введите {(toRead ? "путь к входному файлу " : "только ИМЯ выходного файла ")}" +
                          $"{(books ? "с книгами " : "с читателями ")}\n" +
                          "ИЛИ нажмите Enter, чтобы использовать данные по умолчанию:   ");
            while (true)
            {
                try
                {
                    string path = Console.ReadLine()!.Trim('"');

                    if (string.IsNullOrWhiteSpace(path))
                    {
                        return books ? DataConstants.DefaultBooksFilePath : DataConstants.DefaultReadersFilePath;
                    }

                    if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    {
                        throw new ArgumentException("Путь к файлу содержит недопустимые символы.");
                    }

                    if (!Path.HasExtension(path) || Path.GetExtension(path).ToLower() != ".json")
                    {
                        throw new ArgumentException("Файл должен иметь расширение .json.");
                    }

                    if (toRead && !File.Exists(path))
                    {
                        throw new FileNotFoundException("Исходный файл не найден.", path);
                    }

                    return path;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Write(
                        "Введите путь к файлу ещё раз или нажмите Enter, чтобы использовать файл по умолчанию:   ");
                }
            }
        }

        /// <summary>
        /// Получает запрашиваемый ISBN для книги и валидирует его
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="mainText"></param>
        /// <param name="errorText"></param>
        /// <returns></returns>
        private static string? GetIsbnFromUser(IsbnValidator validator, string mainText, string errorText)
        {
            bool isFirstAttempt = true;
            Console.Write(mainText);
            string isbn = Console.ReadLine()!;
            while (!validator.IsValidIsbn(isbn))
            {
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    if (!isFirstAttempt)
                    {
                        return null;
                    }
                }

                Console.WriteLine(errorText);
                isbn = Console.ReadLine()!;
                isFirstAttempt = false;
            }

            return isbn;
        }


        /// <summary>
        /// Получает строку от пользователя.
        /// </summary>
        /// <param name="mainText">Главный текст, поясняющий что конкретно надо вводить</param>
        /// <param name="errorText">Текст об ошибке, если строка пуста</param>
        /// <param name="canBeNull">Допускается ли null строка</param>
        /// <returns>Введённая строка</returns>
        private static string GetStrFromUser(string mainText, string errorText, bool canBeNull = false)
        {
            while (true)
            {
                Console.Write(mainText);
                string input = Console.ReadLine()!;
                if (canBeNull && string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }

                Console.WriteLine(errorText);
            }
        }

        /// <summary>
        /// Получает от пользователя ID читателя
        /// </summary>
        /// <param name="readerIds"></param>
        /// <returns></returns>
        private static int GetReaderId(int[] readerIds)
        {
            Console.Write("Введите ID читателя:   ");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int userId) && readerIds.Contains(userId))
                {
                    return userId;
                }

                Console.Write("Некорректный ID. Попробуйте снова:   ");
            }
        }

        /// <summary>
        /// Получает год публикации от пользователя
        /// </summary>
        /// <param name="mainText"></param>
        /// <param name="errorText"></param>
        /// <returns></returns>
        private static uint GetPublicationYear(string mainText = "Введите год публикации:  ",
            string errorText = "Год не может быть пустым или отрицательным. Повторите попытку:  ")
        {
            while (true)
            {
                Console.Write(mainText);
                string input = Console.ReadLine()!;
                if (!string.IsNullOrWhiteSpace(input) && uint.TryParse(input, out uint year))
                {
                    return year;
                }

                Console.Write(errorText);
            }
        }

        /// <summary>
        /// Выводит таблицу с читателями и названиями их книг.
        /// </summary>
        /// <param name="readers">Список читателей.</param>
        public static void DisplayReadersAndBooks(List<Reader> readers)
        {
            int nameColumnWidth = readers.Max(r => $"id: {r.Id} {r.FullName}".Length) + 4;
            nameColumnWidth = Math.Max(nameColumnWidth, $"id:   ФИО".Length + 4); // Минимальная ширина для заголовка

            int bookColumnWidth = Math.Max("Название книги".Length + 4, readers
                .SelectMany(r => r.BorrowedBooks.Keys)
                .Max(b => b.Title.Length) + 4);

            Console.WriteLine("\n=== Возврат книги ===\n");
            // Заголовок таблицы
            PrintTableHeader(nameColumnWidth, bookColumnWidth);

            foreach (Reader reader in readers)
            {
                Book? firstBook = reader.BorrowedBooks.Keys.First();
                Console.WriteLine(
                    $"\u2502 {PadRight($"id: {reader.Id} ~ {reader.FullName}", nameColumnWidth)}\u2502 {PadRight(firstBook?.Title, bookColumnWidth)}\u2502");

                foreach (Book? book in reader.BorrowedBooks.Keys.Skip(1))
                {
                    Console.WriteLine(
                        $"\u2502 {PadRight("", nameColumnWidth)}\u2502 {PadRight(book?.Title, bookColumnWidth)}\u2502 ");
                }
            }

            Console.WriteLine($"{new string('\u23af', nameColumnWidth + bookColumnWidth + 5)}");
        }

        // Вспомогательный метод для отрисовки заголовка таблицы
        private static void PrintTableHeader(int nameColumnWidth, int bookColumnWidth)
        {
            string line = new string('\u23af', nameColumnWidth + bookColumnWidth + 5);
            Console.WriteLine("Перед вами табица всех читателей, которые брали книги");
            Console.WriteLine("Введите ID того человека, у которого хотите забрать книгу:\n");
            Console.WriteLine(line);
            Console.Write("\u2502 ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(PadRight("(ID) ФИО", nameColumnWidth));
            Console.ResetColor();
            Console.Write("\u2502 ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(PadRight("Название книги", bookColumnWidth));
            Console.ResetColor();
            Console.WriteLine("\u2502");
            Console.WriteLine(line);
        }

        // Вспомогательный метод для выравнивания текста
        private static string PadRight(string? text, int width)
        {
            if (text == null)
            {
                text = "";
            }

            return text.PadRight(width);
        }


        /// <summary>
        /// Ожидает пока пользователь не нажмёт какую-либо клавишу
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        public static void AwaitExit(string text = "\nНажмите Enter, чтобы продолжить.",
            ConsoleKey key = ConsoleKey.Enter)
        {
            Console.Write(text);
            while (Console.ReadKey().Key != key)
            {
            }
        }
    }
}