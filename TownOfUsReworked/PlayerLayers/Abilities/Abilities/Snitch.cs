namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Snitch : Ability
{
    [ToggleOption]
    public static bool SnitchKnows = true;

    [ToggleOption]
    public static bool SnitchSeesNeutrals = false;

    [ToggleOption]
    public static bool SnitchSeesCrew = false;

    [ToggleOption]
    public static bool SnitchSeesRoles = false;

    [NumberOption(1, 5, 1)]
    public static Number SnitchTasksRemaining = 1;

    [ToggleOption]
    public static bool SnitchSeesTargetsInMeeting = true;

    [ToggleOption]
    public static bool SnitchSeesTraitor = true;

    [ToggleOption]
    public static bool SnitchSeesFanatic = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Snitch : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Snitch;
    public override Func<string> Description => () => "- You can finish your tasks to get information on who's evil";
    public override bool Hidden => !SnitchKnows && !TasksDone && !Dead;

    public override void UponTaskComplete(uint taskId)
    {
        var local = CustomPlayer.Local.GetRole();

        if (TasksLeft == SnitchTasksRemaining)
        {
            if (Local)
                Flash(Color);
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.GetFaction() == Faction.Neutral && SnitchSeesNeutrals))
            {
                Flash(Color);
                local.AllArrows.Add(PlayerId, new(CustomPlayer.Local, Player, Color));
            }
        }
        else if (TasksDone)
        {
            if (Local)
            {
                Flash(UColor.green);
                AllPlayers().Where(x => x.GetFaction() is Faction.Intruder or Faction.Syndicate || (x.GetFaction() == Faction.Neutral && SnitchSeesNeutrals)).ForEach(x =>
                    local.AllArrows.Add(x.PlayerId, new(Player, x, Color)));
            }
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.GetFaction() == Faction.Neutral && SnitchSeesNeutrals))
            {
                Flash(UColor.red);
            }
        }
    }
}