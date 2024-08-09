namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Tunneler : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Tunneler : CustomColorManager.Ability;
    public override string Name => "Tunneler";
    public override LayerEnum Type => LayerEnum.Tunneler;
    public override Func<string> Description => () => "- You can finish tasks to be able to vent";
    public override bool Hidden => !CustomGameOptions.TunnelerKnows && !TasksDone && !Dead;
}