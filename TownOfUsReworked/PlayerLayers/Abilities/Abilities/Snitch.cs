namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Snitch : Ability
{
    public override UColor Color => ClientGameOptions.CustomAbColors ? CustomColorManager.Snitch : CustomColorManager.Ability;
    public override string Name => "Snitch";
    public override LayerEnum Type => LayerEnum.Snitch;
    public override Func<string> Description => () => "- You can finish your tasks to get information on who's evil";
    public override bool Hidden => !CustomGameOptions.TraitorKnows && !TasksDone && !Dead;
}