namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Snitch : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Snitch : CustomColorManager.Ability;
    public override string Name => "Snitch";
    public override LayerEnum Type => LayerEnum.Snitch;
    public override Func<string> Description => () => "- You can finish your tasks to get information on who's evil";
    public override bool Hidden => !CustomGameOptions.TraitorKnows && !TasksDone && !Dead;
}