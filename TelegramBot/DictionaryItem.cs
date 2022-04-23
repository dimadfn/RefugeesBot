namespace TelegramBot
{
    internal class DictionaryItem
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string[] MandatoryKeywords { get; set; }
        public string[] OptionalKeywords { get; set; }
        public int AtLeastCount { get; set; }
        public string[] NotKeywords { get; set; }
    }
}
