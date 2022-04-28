namespace TelegramBot
{
    internal class Suggestion
    {
        private readonly DictionaryItem _rule;

        public Suggestion(DictionaryItem rule)
        {
            _rule = rule;
        }

        public string RuleName => _rule.Name;

        public string Message =>
            $"Добрый день! Возможно вам поможет закреплённый документ, раздел <a href=\"{_rule.Url}\">{_rule.Message}</a>";
    }
}
