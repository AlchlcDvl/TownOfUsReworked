namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Tiebreaker : Ability
{
    public override Color Color => ClientGameOptions.CustomAbColors ? Colors.Tiebreaker : Colors.Ability;
    public override string Name => "Tiebreaker";
    public override LayerEnum Type => LayerEnum.Tiebreaker;
    public override Func<string> Description => () => "- Your votes break ties";
    public override bool Hidden => !CustomGameOptions.TiebreakerKnows && !IsDead;

    public Tiebreaker(PlayerControl player) : base(player) {}
}