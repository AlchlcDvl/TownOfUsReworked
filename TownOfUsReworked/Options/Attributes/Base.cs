namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class OptionAttribute : Attribute
{
    public bool All { get; set; }

    protected abstract Option BaseSetUpOption();

    public Option Set(MemberInfo member, BaseHeaderOption header, bool clientOnly)
    {
        var option = BaseSetUpOption();
        option.All = All;
        option.Set(member, header, clientOnly);
        return option;
    }
}