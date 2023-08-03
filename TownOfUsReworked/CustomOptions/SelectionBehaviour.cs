namespace TownOfUsReworked.CustomOptions
{
    public class SelectionBehaviour
    {
        public readonly string Title;
        public readonly Func<bool> OnClick;
        public readonly bool DefaultValue;

        public SelectionBehaviour(string title, Func<bool> onClick, bool defaultValue)
        {
            Title = title;
            OnClick = onClick;
            DefaultValue = defaultValue;
        }
    }
}