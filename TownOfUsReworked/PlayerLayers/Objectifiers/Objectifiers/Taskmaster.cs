namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Taskmaster : Objectifier
{
    public bool Revealed => TasksLeft <= CustomGameOptions.TMTasksRemaining;
    public bool WinTasksDone { get; set; }

    public override Color Color => ClientGameOptions.CustomObjColors ? Colors.Taskmaster : Colors.Objectifier;
    public override string Name => "Taskmaster";
    public override string Symbol => "µ";
    public override LayerEnum Type => LayerEnum.Taskmaster;
    public override Func<string> Description => () => "- Finish your tasks before the game ends";

    public Taskmaster(PlayerControl player) : base(player) {}
}