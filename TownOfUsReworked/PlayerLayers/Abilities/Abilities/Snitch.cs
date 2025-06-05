namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(LayerEnum.Snitch)]
public sealed class Snitch : Ability
{
    [ToggleOption]
    private static bool SnitchKnows = true;

    [ToggleOption]
    public static bool SnitchSeesOutcasts = false;

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

    protected override UColor MainColor => CustomColorManager.Snitch;
    public override LayerEnum Type => LayerEnum.Snitch;
    public override Func<string> Description => () => "- You can finish your tasks to get information on who's evil";
    public override bool Hidden => !SnitchKnows && !TasksDone && !Dead;

    public override void UponTaskComplete(uint taskId)
    {
        var local = LayerHandler.Handlers[LocalPlayer.PlayerId];

        if (TasksLeft == SnitchTasksRemaining)
        {
            if (Local)
                Flash(Color);
            else if (LocalPlayer.GetFaction() is not (Faction.Crew or Faction.Outcast) || (LocalPlayer.GetFaction() == Faction.Outcast && SnitchSeesOutcasts))
            {
                Flash(Color);
                local.AllArrows.Add(PlayerId, new(LocalPlayer, Player, Color));
            }
        }
        else if (TasksDone)
        {
            if (Local)
            {
                Flash(UColor.green);
                AllPlayers().Where(x => x.GetFaction().IsFactionedEvil() || (x.GetFaction() == Faction.Outcast && SnitchSeesOutcasts)).Do(x =>
                    local.AllArrows.Add(x.PlayerId, new(Player, x, Color)));
            }
            else if (LocalPlayer.GetFaction() is not (Faction.Crew or Faction.Outcast) || (LocalPlayer.GetFaction() == Faction.Outcast && SnitchSeesOutcasts))
                Flash(UColor.red);
        }
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!TasksDone || (meeting && !SnitchSeesTargetsInMeeting))
            return;

        var role = handler.CurrentRole;

        if (SnitchSeesRoles)
        {
            if ((role.Faction != Faction.Outcast || !SnitchSeesOutcasts) && (role.Faction != Faction.Crew || !SnitchSeesCrew))
                return;

            color = role.Color;
            name += $"\n{role.Name}";
            revealed = true;
        }
        else if (role.Faction is not (Faction.Crew or Faction.Outcast) || (role.Faction == Faction.Outcast && SnitchSeesOutcasts) || (role.Faction == Faction.Crew && SnitchSeesCrew))
        {
            if (handler.CurrentDisposition is FactionChanger { SnitchReveals: false })
            {
                color = CustomColorManager.Crew;
                name += "\nCrew";
            }
            else
            {
                color = role.FactionColor;
                name += $"\n{role.FactionName}";
            }

            revealed = true;
        }
    }
}