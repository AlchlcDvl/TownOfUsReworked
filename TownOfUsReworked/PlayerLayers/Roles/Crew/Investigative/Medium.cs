namespace TownOfUsReworked.PlayerLayers.Roles;

// TODO: Implement seancing
[LayerHeaderOption(LayerEnum.Medium)]
public sealed class Medium : Crew, IShaman
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number MediateCd = 25;

    // [NumberOption(10f, 60f, 2.5f, Format.Time)]
    // public static Number SeanceCd = 25;

    // [NumberOption(5f, 30f, 1f, Format.Time)]
    // public static Number SeanceDur = 10;

    [ToggleOption]
    public static bool ShowMediatePlayer = true;

    [StringOption<ShowMediumToDead>]
    public static ShowMediumToDead ShowMediumToDead = ShowMediumToDead.Never;

    [StringOption<DeadRevealed>]
    public static DeadRevealed DeadRevealed = DeadRevealed.Oldest;

    private Dictionary<byte, PlayerArrow> MediateArrows { get; set; }
    private CustomButton MediateButton { get; set; }
    // private CustomButton SeanceButton { get; set; }
    // public bool HasSeanced { get; set; }
    public HashSet<byte> MediatedPlayers { get; } = [];

    protected override UColor MainColor => CustomColorManager.Medium;
    public override LayerEnum Type => LayerEnum.Medium;
    public override Func<string> StartText { get; } = () => "<size=80%>Spooky Scary Ghosties Send Shivers Down Your Spine</size>";
    public override Func<string> Description => () => "- You can mediate which makes ghosts visible to you" + (ShowMediumToDead == ShowMediumToDead.Never ? "" : ("\n- When mediating, dead " +
        "players will be able to see you"));

    protected override void Init()
    {
        base.Init();
        MediatedPlayers.Clear();
        MediateArrows = [];
        Alignment = Alignment.Investigative;
        MediateButton ??= new(this, "MEDIATE", new SpriteName("Mediate"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Mediate, new Cooldown(MediateCd));
        // SeanceButton ??= new(this, "SEANCE", new SpriteName("Seance"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Seance, new Cooldown(SeanceCd), 1,
        //     new PostDeath(true));
    }

    public override void Reset(bool meeting, bool start) => ClearArrows();

    // private void Seance() { /*Currently blank, gonna work on this later*/ }
    // Can you believe this guy? Over a year and this mofo still hasn't worked on it :skull:

    public override void ClearArrows()
    {
        base.ClearArrows();
        MediateArrows.Values.DestroyAll();
        MediateArrows.Clear();
        MediatedPlayers.Clear();
    }

    public override void UpdatePlayer(PlayerControl __instance)
    {
        if (Dead || !MediateArrows.TryGetValue(__instance.PlayerId, out var arrow))
            return;

        arrow?.Update(__instance.GetPlayerColor(false, !ShowMediatePlayer));

        if (ShowMediatePlayer)
            return;

        __instance.CustomSetOutfit(CustomPlayerOutfitType.Camouflage, BlankOutfit(__instance));
        PlayerMaterial.SetColors(UColor.grey, __instance.MyRend());
    }

    private void Mediate()
    {
        MediateButton.StartCooldown();
        var playersDead = KilledPlayers.Clone();

        if (!playersDead.Any())
            return;

        var bodies = AllBodies();

        switch (DeadRevealed)
        {
            case DeadRevealed.Random:
            {
                MediatePlayer(playersDead.Random(), bodies);
                break;
            }
            case DeadRevealed.Everyone:
            {
                playersDead.Do(x => MediatePlayer(x, bodies));
                break;
            }
            default:
            {
                if (DeadRevealed == DeadRevealed.Newest)
                    playersDead = playersDead.Reverse();

                MediatePlayer(playersDead.First(), bodies);
                break;
            }
        }
    }

    private void MediatePlayer(DeadPlayer dead, IEnumerable<DeadBody> bodies)
    {
        if (!bodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            return;

        MediateArrows.Add(dead.PlayerId, new(Player, PlayerById(dead.PlayerId), Color, skipBody: true));
        MediatedPlayers.Add(dead.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, dead.PlayerId);
    }

    public override void ReadRPC(NetData reader)
    {
        var playerid2 = reader.ReadByte();
        MediatedPlayers.Add(playerid2);

        if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.Local.HasDied() && ShowMediumToDead == ShowMediumToDead.AllDead))
            CustomPlayer.Local.GetRole().DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Player, Color, skipBody: true));
    }
}