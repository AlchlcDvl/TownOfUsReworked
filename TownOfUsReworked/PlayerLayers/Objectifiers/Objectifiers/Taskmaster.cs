namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Taskmaster : Objectifier
{
    [NumberOption(MultiMenu.LayerSubOptions, 1, 5, 1)]
    public static Number TMTasksRemaining { get; set; } = new(1);

    public bool WinTasksDone { get; set; }

    public override UColor Color => ClientOptions.CustomObjColors ? CustomColorManager.Taskmaster : CustomColorManager.Objectifier;
    public override string Name => "Taskmaster";
    public override string Symbol => "µ";
    public override LayerEnum Type => LayerEnum.Taskmaster;
    public override Func<string> Description => () => "- Finish your tasks before the game ends";
}