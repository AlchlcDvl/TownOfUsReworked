namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Tiebreaker : Ability
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TiebreakerKnows { get; set; } = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Tiebreaker : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Tiebreaker;
    public override Func<string> Description => () => "- Your votes break ties";
    public override bool Hidden => !TiebreakerKnows && !Dead;
}