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
    public static int SnitchTasksRemaining { get; set; } = 1;

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
}