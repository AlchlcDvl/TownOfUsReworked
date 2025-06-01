namespace TownOfUsReworked.Options.Attributes;

public sealed class ToggleOptionAttribute : OptionAttribute<ToggleOption>
{
    protected override ToggleOption SetUpOption() => new();
}