namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Taskmaster)]
public sealed class Taskmaster : Disposition
{
    [NumberOption(1, 5, 1)]
    public static Number TMTasksRemaining = 1;

    protected override UColor MainColor => CustomColorManager.Taskmaster;
    public override string Symbol => "µ";
    public override LayerEnum Type => LayerEnum.Taskmaster;
    public override Func<string> Description => () => "- Finish your tasks before the game ends";

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == TMTasksRemaining)
        {
            var role = CustomPlayer.Local.GetRole();

            if (Local || role.Faction == Faction.Crew || role.Alignment is Alignment.Benign or Alignment.Evil)
                Flash(Color);
            else if (role.Faction is Faction.Intruder or Faction.Syndicate || role.Alignment is Alignment.Killing or Alignment.Neophyte or Alignment.Proselyte or Alignment.Harbinger or
                Alignment.Apocalypse)
            {
                Flash(Color);
                role.AllArrows.Add(PlayerId, new(CustomPlayer.Local, Player, Color));
            }
        }
        else if (TasksDone && Local)
            Flash(Color);

        if (!AmongUsClient.Instance || !TasksDone)
            return;

        WinState = WinLose.TaskmasterWins;
        Winner = true;
        CallRpc(CustomRPC.WinLose, WinLose.TaskmasterWins, this);
    }
}