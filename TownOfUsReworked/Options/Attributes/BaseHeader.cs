namespace TownOfUsReworked.Options.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class BaseHeaderOptionAttribute(MultiMenu menu) : OptionAttribute<BaseHeaderOption>
{
    protected MultiMenu Menu { get; } = menu;
    public bool IsOpen { get; init; }
    public bool ClientOnly { get; init; }

    /// <summary>
    /// Sets up the header and its grouped options from the provided type.
    /// </summary>
    public void SetTypeAndOptions(Type type)
    {
        var header = SetUpOption();
        header.ClientOnly = ClientOnly;
        header.All = All;
        header.SetTypeAndOptions(type);
        header.Value = IsOpen;
    }
}

// ReSharper disable once UnusedTypeParameter
public abstract class BaseHeaderOptionAttribute<T>(MultiMenu menu) : BaseHeaderOptionAttribute(menu) where T : BaseHeaderOption;