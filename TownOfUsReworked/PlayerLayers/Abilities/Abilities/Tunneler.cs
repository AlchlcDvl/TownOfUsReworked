namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.Tunneler)]
public sealed class Tunneler : Ability
{
    [ToggleOption]
    private static bool TunnelerKnows = true;

    protected override UColor MainColor => CustomColorManager.Tunneler;
    public override Layer Type => Layer.Tunneler;
    public override string Description => "- You can finish tasks to be able to vent";
    public override bool Hidden => !TunnelerKnows && !TasksDone && !Dead;
    public override bool CanVent => TasksDone;
}