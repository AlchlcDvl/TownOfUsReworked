namespace TownOfUsReworked.Options;

public sealed class HeaderOptionAttribute(MultiMenu menu) : BaseHeaderOptionAttribute<HeaderOption>(menu)
{
    protected override HeaderOption SetUpOption() => new(Menu);
}