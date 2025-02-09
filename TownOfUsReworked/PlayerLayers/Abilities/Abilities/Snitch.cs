namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Snitch : Ability
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SnitchKnows { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SnitchSeesNeutrals { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SnitchSeesCrew { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SnitchSeesRoles { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 5, 1)]
    public static Number SnitchTasksRemaining { get; set; } = new(1);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SnitchSeesTargetsInMeeting { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SnitchSeesTraitor { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SnitchSeesFanatic { get; set; } = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Snitch : CustomColorManager.Ability;
    public override string Name => "Snitch";
    public override LayerEnum Type => LayerEnum.Snitch;
    public override Func<string> Description => () => "- You can finish your tasks to get information on who's evil";
    public override bool Hidden => !SnitchKnows && !TasksDone && !Dead;

    public override void UponTaskComplete(uint taskId)
    {
        base.UponTaskComplete(taskId);

        if (TasksLeft == SnitchTasksRemaining)
        {
            if (CustomPlayer.Local == Player)
                Flash(Color);
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or
                Alignment.NeutralPros && SnitchSeesNeutrals))
            {
                Flash(Color);
                Role.LocalRole.AllArrows.Add(PlayerId, new(CustomPlayer.Local, Color));
            }
        }
        else if (TasksDone)
        {
            if (Local)
            {
                Flash(UColor.green);
                AllPlayers().Where(x => x.GetFaction() is Faction.Intruder or Faction.Syndicate || (x.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or Alignment.NeutralPros &&
                    SnitchSeesNeutrals)).ForEach(x => Role.LocalRole.AllArrows.Add(x.PlayerId, new(Player, Color)));
            }
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or
                Alignment.NeutralPros && SnitchSeesNeutrals))
            {
                Flash(UColor.red);
            }
        }
    }
}