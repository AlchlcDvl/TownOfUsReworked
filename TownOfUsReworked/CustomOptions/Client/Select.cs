namespace TownOfUsReworked.CustomOptions;

public class SelectOption
{
    public readonly string Title;
    public readonly Func<bool> OnClick;
    public readonly bool DefaultValue;

    public SelectOption(string title, Func<bool> onClick, bool defaultValue)
    {
        Title = title;
        OnClick = onClick;
        DefaultValue = defaultValue;
    }
}