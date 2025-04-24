namespace Data
{
    public static class DataConstants
    {
        // Список допустимых жанров
        public static readonly string[] Genres =
        [
            "Роман", "Фантастика", "Антиутопия", "Исторический роман", "Фэнтези", "Сказка", "Магический реализм",
            "Приключения", "Трагедия", "Сатира"
        ];

        // Список стран (код страны -> название)
        public static readonly Dictionary<string, string> Countries = new Dictionary<string, string>
        {
            { "978", "Русскоязычные страны" },
            { "979", "США" },
            { "977", "Великобритания" },
            { "976", "Франция" },
            { "975", "Хорватия" },
            { "974", "Колумбия" }, 
            { "973", "Испания" }
        };

        // Список издательств (код издательства -> название)
        public static readonly Dictionary<string, string> Publishers = new Dictionary<string, string>
        {
            { "1234", "ЭКСМО" },
            { "5678", "АСТ" },
            { "9101", "Питер" },
            { "1122", "Манн, Иванов и Фербер" },
            { "3344", "Олимп-Бизнес" },
            { "4455", "Художественная литература" }, 
            { "5566", "Молодая гвардия" },         
            { "6677", "Secker & Warburg" },          
            { "7788", "Русская классика" },         
            { "8899", "Bloomsbury" },               
            { "9900", "Reynal & Hitchcock" },        
            { "1011", "Editorial Sudamericana" },  
            { "1213", "Olympia Press" },             
            { "1314", "Chatto & Windus" },           
            { "1415", "Francisco de Robles" },       
            { "1516", "T. Egerton" },               
            { "1617", "Harper & Brothers" },         
            { "1718", "Allen & Unwin" },            
            { "1819", "Nicholas Ling" }              
        };

        public static readonly string UnderLine =
            "\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af" +
            "\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af\u23af";

        // Пути к файлам по умолчанию
        public static readonly string DefaultBooksFilePath = "books.json";
        public static readonly string DefaultReadersFilePath = "readers.json";
    }
}