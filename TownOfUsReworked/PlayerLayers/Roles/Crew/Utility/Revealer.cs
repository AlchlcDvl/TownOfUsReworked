namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Revealer)]
public sealed class Revealer : CUtility, IGhosty
{
    [NumberOption(1, 10, 1)]
    private static Number RevealerTasksRemainingClicked = 5;

    [NumberOption(1, 5, 1)]
    private static Number RevealerTasksRemainingAlert = 1;

    [ToggleOption]
    public static bool RevealerRevealsOutcasts = false;

    [ToggleOption]
    public static bool RevealerRevealsCrew = false;

    [ToggleOption]
    public static bool RevealerRevealsRoles = false;

    [ToggleOption]
    public static bool RevealerRevealsTraitor = false;

    [ToggleOption]
    public static bool RevealerRevealsFanatic = false;

    [StringOption<RevealerCanBeClickedBy>]
    private static RevealerCanBeClickedBy RevealerCanBeClickedBy = RevealerCanBeClickedBy.Everyone;

    public bool Caught { get; set; }
    public bool Revealed { get; private set; }
    public Crew FormerRole;
    public Vector3 LastPosition { get; set; }

    protected override UColor MainColor => CustomColorManager.Revealer;
    public override Layer Type => Layer.Revealer;
    public override string StartText => "OOOOOOO";
    public override string Description => "- You can reveal evils players to the <#8CFFFFFF>Crew</color> once you finish your tasks without getting clicked.";

    public override void Init()
    {
        RemoveTasks(Player);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    public override void BeforeMeeting()
    {
        if (!UninteractablePlayers.ContainsKey(PlayerId))
            LastPosition = Player.transform.position;
    }

    public override void UponTaskComplete(uint taskId)
    {
        var isEvil = LocalPlayer.GetFaction().IsFactionedEvil() || ((LocalPlayer.GetAlignment() is Alignment.Neophyte or Alignment.Proselyte || LocalPlayer.GetRole() is OKilling or Neophyte) &&
            RevealerRevealsOutcasts);

        if (TasksLeft == RevealerTasksRemainingAlert && !Caught)
        {
            if (Local)
                Flash(Color);
            else if (isEvil)
            {
                Revealed = true;
                Flash(Color);
                LayerHandler.Handlers[LocalPlayer.PlayerId].DeadArrows.Add(PlayerId, new(LocalPlayer, Player, Color));
            }
        }
        else if (isEvil)
            Flash(Color);
    }

    public bool CanBeClicked(PlayerControl clicker)
    {
        var done = TasksLeft <= RevealerTasksRemainingClicked;

        if (RevealerCanBeClickedBy == RevealerCanBeClickedBy.Everyone)
            return done;

        if ((RevealerCanBeClickedBy == RevealerCanBeClickedBy.EvilsOnly && !clicker.GetFaction().IsFactionedEvil()) || (clicker.Is(Faction.Crew) && RevealerCanBeClickedBy ==
            RevealerCanBeClickedBy.NonCrew))
        {
            return false;
        }

        return done;
    }

    public override void UponRoleChanged(Role former) => FormerRole = (Crew)former;
}