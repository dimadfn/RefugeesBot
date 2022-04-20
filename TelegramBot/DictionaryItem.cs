namespace TelegramBot
{
    internal class DictionaryItem
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string[] MandatoryKeyvords { get; set; }
        public string[] AtLeastKeyvords { get; set; }
        public int AtLeastCount { get; set; }
        public string[] NotKeyvords { get; set; }
    }
}
