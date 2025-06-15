namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(Layer.Taskmaster)]
public sealed class Taskmaster : Disposition
{
    [NumberOption(1, 5, 1)]
    private static Number TMTasksRemaining = 1;

    protected override UColor MainColor => CustomColorManager.Taskmaster;
    public override string Symbol => "µ";
    public override Layer Type => Layer.Taskmaster;
    public override string Description => "- Finish your tasks before the game ends";

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == TMTasksRemaining)
        {
            var role = LayerHandler.Handlers[LocalPlayer.PlayerId];
            Flash(Color);

            if (role.CurrentFaction.IsFactionedEvil() || role.CurrentRole.Alignment is Alignment.Killing or Alignment.Neophyte or Alignment.Proselyte)
                role.AllArrows.Add(PlayerId, new(LocalPlayer, Player, Color));
        }
        else if (TasksDone && Local)
            Flash(Color);
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!TasksDone)
            return;

        WinState = WinLose.TaskmasterWins;
        winnerIds.Add(PlayerId);
    }
}