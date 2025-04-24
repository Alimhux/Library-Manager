namespace Utilities
{
    /// <summary>
    /// Валидатор ISBN для книги
    /// </summary>
    public class IsbnValidator
    {
        /// <summary>
        /// Возвращает, валидный ли isbn у книги
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public bool IsValidIsbn(string isbn)
        {
            string cleanIsbn = isbn.Replace("-", "").Replace(" ", "").ToUpper();
            
            if (cleanIsbn.Length != 10 && cleanIsbn.Length != 13)
            {
                return false;
            }

            return cleanIsbn.Length switch
            {
                10 => ValidateIsbn10(cleanIsbn),
                13 => ValidateIsbn13(cleanIsbn),
                _ => false
            };
        }

        /// <summary>
        /// Проверка для кода длинной 10 символов
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        private static bool ValidateIsbn10(string isbn)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(isbn[i]))
                {
                    return false;
                }
            }

            if (!(char.IsDigit(isbn[9]) || isbn[9] == 'X'))
            {
                return false;
            }

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                int value = (i == 9 && isbn[i] == 'X') ? 10 : isbn[i] - '0';
                sum += value * (10 - i);
            }

            return sum % 11 == 0;
        }

        /// <summary>
        /// Проверка для кода длинной 13 символов
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        private static bool ValidateIsbn13(string isbn)
        {
            if (!isbn.All(char.IsDigit))
            {
                return false;
            }

            string prefix = isbn[..3];
            if (prefix != "978" && prefix != "979")
            {
                return false;
            }

            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = isbn[i] - '0';
                sum += digit * (i % 2 == 0 ? 1 : 3);
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == isbn[12] - '0';
        }
    }
}