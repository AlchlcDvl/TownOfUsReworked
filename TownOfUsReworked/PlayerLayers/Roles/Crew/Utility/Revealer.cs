namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Revealer)]
public sealed class Revealer : Crew, IGhosty
{
    [NumberOption(1, 10, 1)]
    public static Number RevealerTasksRemainingClicked = 5;

    [NumberOption(1, 5, 1)]
    private static Number RevealerTasksRemainingAlert = 1;

    [ToggleOption]
    public static bool RevealerRevealsNeutrals = false;

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
    public bool Faded { get; set; }
    public Role FormerRole { get; init; }

    protected override UColor MainColor => CustomColorManager.Revealer;
    public override LayerEnum Type => LayerEnum.Revealer;
    public override Func<string> StartText => () => "OOOOOOO";
    public override Func<string> Description => () => "- You can reveal evils players to the <#8CFFFFFF>Crew</color> once you finish your tasks without getting clicked.";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
        RemoveTasks(Player);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == RevealerTasksRemainingAlert && !Caught)
        {
            if (Local)
                Flash(Color);
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || ((CustomPlayer.Local.GetAlignment() is Alignment.Neophyte or Alignment.Proselyte or
                Alignment.Harbinger or Alignment.Apocalypse || CustomPlayer.Local.GetRole() is NKilling) && RevealerRevealsNeutrals))
            {
                Revealed = true;
                Flash(Color);
                CustomPlayer.Local.GetRole().DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Player, Color));
            }
        }
        else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || ((CustomPlayer.Local.GetAlignment() is Alignment.Neophyte or Alignment.Proselyte or
            Alignment.Harbinger or Alignment.Apocalypse || CustomPlayer.Local.GetRole() is NKilling) && RevealerRevealsNeutrals))
        {
            Flash(Color);
        }
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();
        (this as IGhosty).UpdateGhost();
    }
}