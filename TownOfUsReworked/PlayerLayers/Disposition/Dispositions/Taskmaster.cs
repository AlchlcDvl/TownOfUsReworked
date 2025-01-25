namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Taskmaster : Disposition
{
    [NumberOption(1, 5, 1)]
    public static Number TMTasksRemaining = 1;

    public bool WinTasksDone { get; set; }

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Taskmaster : CustomColorManager.Disposition;
    public override string Symbol => "µ";
    public override LayerEnum Type => LayerEnum.Taskmaster;
    public override Func<string> Description => () => "- Finish your tasks before the game ends";

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == TMTasksRemaining)
        {
            if (Local || CustomPlayer.Local.Is(Faction.Crew) || CustomPlayer.Local.GetAlignment() is Alignment.Benign or Alignment.Evil)
                Flash(Color);
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || CustomPlayer.Local.GetAlignment() is Alignment.Killing or Alignment.Neophyte or
                Alignment.Proselyte or Alignment.Harbinger or Alignment.Apocalypse)
            {
                Flash(Color);
                CustomPlayer.Local.GetRole().AllArrows.Add(PlayerId, new(CustomPlayer.Local, Player, Color));
            }
        }
        else if (TasksDone)
        {
            if (Local)
                Flash(Color);

            WinTasksDone = true;
        }
    }

    public override void CheckWin()
    {
        if (TasksDone)
        {
            WinState = WinLose.TaskmasterWins;
            Winner = true;
            CallRpc(CustomRPC.WinLose, WinLose.TaskmasterWins, this);
        }
    }
}