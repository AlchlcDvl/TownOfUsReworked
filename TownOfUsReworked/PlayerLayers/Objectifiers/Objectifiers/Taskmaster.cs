namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Taskmaster : Objectifier
{
    public bool WinTasksDone { get; set; }

    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Taskmaster : CustomColorManager.Objectifier;
    public override string Name => "Taskmaster";
    public override string Symbol => "µ";
    public override LayerEnum Type => LayerEnum.Taskmaster;
    public override Func<string> Description => () => "- Finish your tasks before the game ends";
}