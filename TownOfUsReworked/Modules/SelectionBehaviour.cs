namespace TownOfUsReworked.Modules
{
    public class SelectionBehaviour
    {
        public string Title;
        public Func<bool> OnClick;
        public bool DefaultValue;

        public SelectionBehaviour(string title, Func<bool> onClick, bool defaultValue)
        {
            Title = title;
            OnClick = onClick;
            DefaultValue = defaultValue;
        }
    }
}