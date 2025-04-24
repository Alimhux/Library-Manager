namespace Utilities
{
    public static class MenuWorker
    {
        /// <summary>
        /// Получает от пользователя необходимый пунк меню
        /// </summary>
        /// <param name="mainTile"></param>
        /// <param name="postTile"></param>
        /// <param name="titles"></param>
        /// <returns></returns>
        public static int DisplayOptionsMenu(string mainTile, string postTile, params string?[] titles)
        {
            Console.Clear();
            DrawMenuFrame(mainTile, postTile, true, titles);
            return GetUserOption(titles.Length);
        }
        
        /// <summary>
        /// Отрисовывает каркас меню
        /// </summary>
        /// <param name="mainTile"></param>
        /// <param name="postTile"></param>
        /// <param name="withCursor"></param>
        /// <param name="titles"></param>
        internal static void DrawMenuFrame(string mainTile, string postTile, bool withCursor = true, params string?[] titles)
        {
            Console.WriteLine(mainTile + '\n');

            foreach (string? t in titles)
            {
                Console.WriteLine("  " + t);
            }

            Console.SetCursorPosition(0, 2);
            Console.Write($"{(withCursor ? ">" : "")}");

            Console.SetCursorPosition(0, titles.Length + 3);
            Console.WriteLine(postTile);
        }
        
        /// <summary>
        /// Получает номер сверху выбранной опции
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private static int GetUserOption(int len)
        {
            int pos = 1;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                Console.SetCursorPosition(0, pos + 1);
                Console.Write(' ');

                if (key.Key == ConsoleKey.UpArrow)
                {
                    pos += pos > 1 ? -1 : 0;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    pos += pos < len ? 1 : 0;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    return pos;
                }

                Console.SetCursorPosition(0, pos + 1);
                Console.Write('>');
                Console.SetCursorPosition (0, len + 4);
            }
        }
    }
}