namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Tunneler : Ability
{
    [ToggleOption]
    public static bool TunnelerKnows = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Tunneler : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Tunneler;
    public override Func<string> Description => () => "- You can finish tasks to be able to vent";
    public override bool Hidden => !TunnelerKnows && !TasksDone && !Dead;
}