namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Insider : Ability
{
    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Insider : CustomColorManager.Ability;
    public override string Name => "Insider";
    public override LayerEnum Type => LayerEnum.Insider;
    public override Func<string> Description => () => "- You can finish your tasks to see the votes of others";
    public override bool Hidden => !CustomGameOptions.InsiderKnows && !TasksDone && !Dead;
}