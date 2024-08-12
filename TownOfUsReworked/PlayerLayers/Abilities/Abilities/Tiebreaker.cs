namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Tiebreaker : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Tiebreaker : CustomColorManager.Ability;
    public override string Name => "Tiebreaker";
    public override LayerEnum Type => LayerEnum.Tiebreaker;
    public override Func<string> Description => () => "- Your votes break ties";
    public override bool Hidden => !CustomGameOptions.TiebreakerKnows && !Dead;
}