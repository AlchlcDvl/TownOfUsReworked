namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Multitasker : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Multitasker : CustomColorManager.Ability;
    public override string Name => "Multitasker";
    public override LayerEnum Type => LayerEnum.Multitasker;
    public override Func<string> Description => () => "- Your task windows are transparent";
}