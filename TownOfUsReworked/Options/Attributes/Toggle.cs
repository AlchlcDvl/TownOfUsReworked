namespace TownOfUsReworked.Options.Attributes;

public sealed class ToggleOptionAttribute : OptionAttribute<ReworkedToggleOption>
{
    protected override ReworkedToggleOption SetUpOption() => new();
}