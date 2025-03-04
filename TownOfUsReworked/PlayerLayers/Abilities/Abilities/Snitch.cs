namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Snitch : Ability
{
    [ToggleOption]
    private static bool SnitchKnows = true;

    [ToggleOption]
    public static bool SnitchSeesNeutrals = false;

    [ToggleOption]
    private static bool SnitchSeesCrew = false;

    [ToggleOption]
    private static bool SnitchSeesRoles = false;

    [NumberOption(1, 5, 1)]
    public static Number SnitchTasksRemaining = 1;

    [ToggleOption]
    private static bool SnitchSeesTargetsInMeeting = true;

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

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!TasksDone || (meeting && !SnitchSeesTargetsInMeeting))
            return;

        var role = handler.CustomRole;

        if (SnitchSeesRoles)
        {
            if (role.Faction is not (Faction.Syndicate or Faction.Intruder) && (role.Faction != Faction.Neutral || !SnitchSeesNeutrals) && (role.Faction != Faction.Crew || !SnitchSeesCrew))
                return;

            color = role.Color;
            name += $"\n{role.Name}";
            revealed = true;
        }
        else if (role.Faction is Faction.Syndicate or Faction.Intruder || (role.Faction == Faction.Neutral && SnitchSeesNeutrals) || (role.Faction == Faction.Crew && SnitchSeesCrew))
        {
            var disp = handler.CustomDisposition;

            if (!(disp is Traitor && SnitchSeesTraitor) && !(disp is Fanatic && SnitchSeesFanatic))
            {
                color = role.FactionColor;
                name += $"\n{role.FactionName}";
            }
            else
            {
                color = CustomColorManager.Crew;
                name += "\nCrew";
            }

            revealed = true;
        }
    }
}