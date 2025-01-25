namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Tiebreaker : Ability
{
    [ToggleOption]
    public static bool TiebreakerKnows = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Tiebreaker : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Tiebreaker;
    public override Func<string> Description => () => "- Your votes break ties";
    public override bool Hidden => !TiebreakerKnows && !Dead;
}