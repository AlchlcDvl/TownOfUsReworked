namespace TownOfUsReworked.Options;

public sealed class ToggleOptionAttribute : OptionAttribute<ToggleOption>
{
    protected override ToggleOption SetUpOption() => new();
}