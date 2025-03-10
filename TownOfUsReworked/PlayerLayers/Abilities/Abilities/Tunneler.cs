namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(LayerEnum.Tunneler)]
public sealed class Tunneler : Ability
{
    [ToggleOption]
    private static bool TunnelerKnows = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Tunneler : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Tunneler;
    public override Func<string> Description => () => "- You can finish tasks to be able to vent";
    public override bool Hidden => !TunnelerKnows && !TasksDone && !Dead;
}