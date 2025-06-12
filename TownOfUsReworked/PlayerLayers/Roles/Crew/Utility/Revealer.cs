namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Revealer)]
public sealed class Revealer : CUtility, IGhosty
{
    [NumberOption(1, 10, 1)]
    public static Number RevealerTasksRemainingClicked = 5;

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
    public static RevealerCanBeClickedBy RevealerCanBeClickedBy = RevealerCanBeClickedBy.Everyone;

    public bool Caught { get; set; }
    public bool Revealed { get; private set; }
    public Role FormerRole { get; init; }
    public Vector3 LastPosition { get; set; }

    protected override UColor MainColor => CustomColorManager.Revealer;
    public override LayerEnum Type => LayerEnum.Revealer;
    public override Func<string> StartText { get; } = () => "OOOOOOO";
    public override Func<string> Description => () => "- You can reveal evils players to the <#8CFFFFFF>Crew</color> once you finish your tasks without getting clicked.";

    public override void Init()
    {
        base.Init();
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
        if (TasksLeft == RevealerTasksRemainingAlert && !Caught)
        {
            if (Local)
                Flash(Color);
            else if (LocalPlayer.GetFaction() is not (Faction.Crew or Faction.Outcast) || ((LocalPlayer.GetAlignment() is Alignment.Neophyte or Alignment.Proselyte || LocalPlayer.GetRole() is
                OKilling or Neophyte) && RevealerRevealsOutcasts))
            {
                Revealed = true;
                Flash(Color);
                LayerHandler.Handlers[LocalPlayer.PlayerId].DeadArrows.Add(PlayerId, new(LocalPlayer, Player, Color));
            }
        }
        else if (LocalPlayer.GetFaction() is not (Faction.Crew or Faction.Outcast) || ((LocalPlayer.GetAlignment() is Alignment.Neophyte or Alignment.Proselyte || LocalPlayer.GetRole() is OKilling
            or Neophyte) && RevealerRevealsOutcasts))
        {
            Flash(Color);
        }
    }

    public bool CanBeClicked(PlayerControl clicker)
    {
        if ((RevealerCanBeClickedBy == RevealerCanBeClickedBy.EvilsOnly && !(clicker.GetFaction() is not (Faction.Crew or Faction.Outcast))) || (clicker.Is(Faction.Crew) && RevealerCanBeClickedBy ==
            RevealerCanBeClickedBy.NonCrew))
        {
            return false;
        }

        return TasksLeft <= RevealerTasksRemainingClicked;
    }
}