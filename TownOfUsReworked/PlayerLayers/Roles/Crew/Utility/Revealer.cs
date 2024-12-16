namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Revealer : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 1, 10, 1)]
    public static Number RevealerTasksRemainingClicked { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 1, 5, 1)]
    public static Number RevealerTasksRemainingAlert { get; set; } = new(1);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RevealerRevealsNeutrals { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RevealerRevealsCrew { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RevealerRevealsRoles { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RevealerRevealsTraitor { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RevealerRevealsFanatic { get; set; } = false;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static RevealerCanBeClickedBy RevealerCanBeClickedBy { get; set; } = RevealerCanBeClickedBy.Everyone;

    public bool Caught { get; set; }
    public bool Revealed { get; set; }
    public bool Faded { get; set; }
    public Role FormerRole { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Revealer: FactionColor;
    public override string Name => "Revealer";
    public override LayerEnum Type => LayerEnum.Revealer;
    public override Func<string> StartText => () => "OOOOOOO";
    public override Func<string> Description => () => "- You can reveal evils players to the <#8CFFFFFF>Crew</color> once you finish your tasks without getting clicked.";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewUtil;
    }

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;
        var color = new UColor(1f, 1f, 1f, 0f);

        var maxDistance = Ship().MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.GetComponent<Rigidbody2D>().velocity.magnitude;
        color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
        color.a = Mathf.Lerp(color.a, 0, distPercent);

        if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, BlankOutfit(Player));

        Player.MyRend().color = color;
        Player.NameText().color = new(0f, 0f, 0f, 0f);
        Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);

        if (Local)
            Camouflage();
    }

    public void UnFade()
    {
        Player.MyRend().color = UColor.white;
        Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        Faded = false;
        Player.MyPhysics.ResetMoveState();

        if (Local)
            DefaultOutfitAll();
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == RevealerTasksRemainingAlert && !Caught)
        {
            if (Local)
                Flash(Color);
            else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or
                Alignment.NeutralPros && RevealerRevealsNeutrals))
            {
                Revealed = true;
                Flash(Color);
                LocalRole.DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Color));
            }
        }
        else if (TasksDone && !Caught && (Local || CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.GetAlignment() is Alignment.NeutralKill or
            Alignment.NeutralNeo or Alignment.NeutralPros && RevealerRevealsNeutrals)))
        {
            Flash(Color);
        }
    }

    public override void UpdatePlayer()
    {
        if (!Caught)
            Fade();
        else if (Faded)
            UnFade();
    }
}