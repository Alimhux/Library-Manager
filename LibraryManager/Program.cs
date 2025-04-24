using Data;
using Models;
using Services;
using Utilities;

namespace LibraryManager
{
    /// <summary>
    /// Главный класс программы
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Точка входа в программу
        /// </summary>
        private static void Main()
        {
            ConsoleHandler consoleHelper = new ConsoleHandler();
            FileService fileService = new FileService();
            IsbnValidator isbnValidator = new IsbnValidator();
            BookService bookService = new BookService(fileService);
            
            
            string booksFilePath = "", readersFilePath;
            
            // Главный цикл приложения
            bool running = true;
            while (running)
            {
                Console.Clear();
                int choice = MenuWorker.DisplayOptionsMenu("Выберите опцию:",
                    "Перемещайте курсор стрелочками Вниз/Вверх",
                    $"{(bookService.IsLibEmpty ? "Загрузить данные о книгах" : "Изменить данные о книгах")}",
                    $"{(bookService.IsReadersListEmpty ? "Загрузить данные о читателях" : "Изменить данные о читателях")}",
                    "Просмотреть все книги",
                    "Добавить книгу",
                    "Удалить книгу",
                    "Выдать книгу читателю",
                    "Вернуть книгу",
                    "Отфильтровать книги",
                    "Отсротировать книги",
                    "Показать статистику",
                    "Поиск по ISBN",
                    "Выход"
                    );
                try
                {
                    //Вызываем различные методы из ConsoleHandler в зависимоти от выбора пользователя
                    switch (choice)
                    {
                        case 1: 
                            booksFilePath = consoleHelper.GetFileNameOrFilePathFromUser(toRead: true, books: true);
                            bookService.LoadBooks(booksFilePath);
                            break;
        
                        case 2: 
                            readersFilePath = consoleHelper.GetFileNameOrFilePathFromUser(toRead: true, books: false);
                            bookService.LoadReaders(readersFilePath);
                            break;
        
                        case 3:
                            if (!ChekIfDataIsEmpty(bookService))
                            {
                                foreach (Book? book in bookService.GetAllBooks())
                                {
                                    Console.WriteLine(book);
                                    Console.WriteLine(DataConstants.UnderLine + "\n");
                                }
                                ConsoleHandler.AwaitExit();
                            }
                            break;
        
                        case 4:
                            ConsoleHandler.AddBook(bookService, isbnValidator);
                            break;
        
                        case 5:
                            if (!ChekIfDataIsEmpty(bookService))
                            {
                                ConsoleHandler.RemoveBook(bookService);
                            }
                            break;
        
                        case 6:
                            if (!ChekIfDataIsEmpty(bookService, false))
                            {
                                ConsoleHandler.BorrowBook(bookService);
                            }
                            break;
        
                        case 7:
                            if (!ChekIfDataIsEmpty(bookService, false))
                            {
                                ConsoleHandler.ReturnBook(bookService);
                            }
                            break;
        
                        case 8: 
                            if (!ChekIfDataIsEmpty(bookService))
                            {
                                ConsoleHandler.FilterBooks(bookService);
                            }
                            break;
        
                        case 9: 
                            if (!ChekIfDataIsEmpty(bookService))
                            {
                                ConsoleHandler.SortBooks(bookService);
                            }
                            break;
                        case 10: 
                            ConsoleHandler.GetStatistics(bookService, new StatisticService(bookService));
                            break;
                        case 11: 
                            if (!ChekIfDataIsEmpty(bookService))
                            {
                                ConsoleHandler.FindBookByIsbn(bookService, new IsbnSearchService(bookService), isbnValidator);
                            }
                            break;
        
                        case 12:
                            Console.WriteLine("\n=== Завершение работы ===");
                            int whereToSave = MenuWorker.DisplayOptionsMenu("Каким способом сохранить данные?",
                                "Перемещайте курсор стрелочками Вверх/Вниз",
                                "Перезаписать исходный файл.",
                                "Создать новый файл и записать данные в него.");
                            if (whereToSave == 2)
                            {
                                booksFilePath = consoleHelper.GetFileNameOrFilePathFromUser(toRead: false, books: true);
                            }
                            if (ConsoleHandler.FinishWork(bookService, booksFilePath, whereToSave))
                            {
                                running = false;
                            }
                            break;
                    }
                }
                
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                    
            }
        }
        /// <summary>
        /// Проверка на то, пустая ли база данных
        /// </summary>
        /// <param name="bookService"></param>
        /// <param name="onlyLib"></param>
        /// <returns></returns>
        private static bool ChekIfDataIsEmpty(BookService bookService, bool onlyLib = true)
        {
            if (bookService.IsLibEmpty || (bookService.IsReadersListEmpty && !onlyLib))
            {
                Console.WriteLine("\n=== \u20e0 ТАК НИЗЗЯ! ===\n");
                Console.WriteLine($"Список {(bookService.IsLibEmpty ? "книг" : "читателей")} пуст. " +
                                  $"Вернитесь в главное меню и загрузите json-файл с данными ");
                ConsoleHandler.AwaitExit();
                return true;
            }

            return false;
        }
    }
}