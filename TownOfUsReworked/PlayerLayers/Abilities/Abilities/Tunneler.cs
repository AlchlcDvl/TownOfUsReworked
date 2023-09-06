namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Tunneler : Ability
{
    public override Color Color => ClientGameOptions.CustomAbColors ? Colors.Tunneler : Colors.Ability;
    public override string Name => "Tunneler";
    public override LayerEnum Type => LayerEnum.Tunneler;
    public override Func<string> Description => () => "- You can finish tasks to be able to vent";
    public override bool Hidden => !CustomGameOptions.TunnelerKnows && !TasksDone && !IsDead;

    public Tunneler(PlayerControl player) : base(player) {}
}