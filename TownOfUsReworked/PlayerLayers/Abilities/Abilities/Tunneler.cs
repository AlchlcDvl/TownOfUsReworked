namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Tunneler : Ability
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TunnelerKnows { get; set; } = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Tunneler : CustomColorManager.Ability;
    public override string Name => "Tunneler";
    public override LayerEnum Type => LayerEnum.Tunneler;
    public override Func<string> Description => () => "- You can finish tasks to be able to vent";
    public override bool Hidden => !TunnelerKnows && !TasksDone && !Dead;
}