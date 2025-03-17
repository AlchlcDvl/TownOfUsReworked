namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class BaseHeaderOptionAttribute(MultiMenu menu) : OptionAttribute<BaseHeaderOption>
{
    protected MultiMenu Menu { get; } = menu;
    public bool ClientOnly { get; set; }

    /// <summary>
    /// Sets up the header and its grouped options based on the provided type.
    /// </summary>
    /// <param name="type">The type containing the options.</param>
    public void SetTypeAndOptions(Type type)
    {
        var header = SetUpOption();
        header.ClientOnly = ClientOnly;
        header.SetTypeAndOptions(type);
    }
}

// ReSharper disable once UnusedTypeParameter
public abstract class BaseHeaderOptionAttribute<T>(MultiMenu menu) : BaseHeaderOptionAttribute(menu) where T : BaseHeaderOption;