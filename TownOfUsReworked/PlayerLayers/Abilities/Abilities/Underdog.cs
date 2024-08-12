namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Underdog : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Underdog : CustomColorManager.Ability;
    public override string Name => "Underdog";
    public override LayerEnum Type => LayerEnum.Underdog;
    public override Func<string> Description => () => Last(Player) ? "- You have shortened cooldowns" : (CustomGameOptions.UnderdogIncreasedKC ? ("- You have long cooldowns while you're " +
        "not alone") : "- You have short cooldowns when you're alone");
    public override bool Hidden => !CustomGameOptions.TraitorKnows && !Last(Player) && !Dead;
}