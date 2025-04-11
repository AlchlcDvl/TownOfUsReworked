namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(LayerEnum.Multitasker)]
public sealed class Multitasker : Ability
{
    [NumberOption(10f, 80f, 5f, Format.Percent)]
    public static Number Transparency = 50;

    protected override UColor MainColor => CustomColorManager.Multitasker;
    public override LayerEnum Type { get; } = LayerEnum.Multitasker;
    public override Func<string> Description => () => "- Your task windows are transparent";
}