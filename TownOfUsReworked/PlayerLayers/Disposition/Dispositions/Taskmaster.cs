namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Taskmaster : Disposition
{
    [NumberOption(MultiMenu.LayerSubOptions, 1, 5, 1)]
    public static Number TMTasksRemaining { get; set; } = new(1);

    public bool WinTasksDone { get; set; }

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Taskmaster : CustomColorManager.Disposition;
    public override string Name => "Taskmaster";
    public override string Symbol => "µ";
    public override LayerEnum Type => LayerEnum.Taskmaster;
    public override Func<string> Description => () => "- Finish your tasks before the game ends";

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == TMTasksRemaining)
        {
            if (Local || CustomPlayer.Local.Is(Faction.Crew) || CustomPlayer.Local.GetAlignment() is Alignment.NeutralBen or Alignment.NeutralEvil)
                Flash(Color);
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || CustomPlayer.Local.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or
                Alignment.NeutralPros)
            {
                Flash(Color);
                CustomPlayer.Local.GetRole().AllArrows.Add(PlayerId, new(CustomPlayer.Local, Color));
            }
        }
        else if (TasksDone)
        {
            if (Local)
                Flash(Color);

            WinTasksDone = true;
        }
    }
}