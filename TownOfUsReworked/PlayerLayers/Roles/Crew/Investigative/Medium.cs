namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Medium : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number MediateCd { get; set; } = new(25);

    // [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    // public static Number SeanceCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ShowMediatePlayer { get; set; } = true;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static ShowMediumToDead ShowMediumToDead { get; set; } = ShowMediumToDead.Never;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static DeadRevealed DeadRevealed { get; set; } = DeadRevealed.Oldest;

    public Dictionary<byte, PlayerArrow> MediateArrows { get; set; }
    public CustomButton MediateButton { get; set; }
    // public CustomButton SeanceButton { get; set; }
    // public bool HasSeanced { get; set; }

    public List<byte> MediatedPlayers { get; } = [];

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Medium : FactionColor;
    public override LayerEnum Type => LayerEnum.Medium;
    public override Func<string> StartText => () => "<size=80%>Spooky Scary Ghosties Send Shivers Down Your Spine</size>";
    public override Func<string> Description => () => "- You can mediate which makes ghosts visible to you" + (ShowMediumToDead == ShowMediumToDead.Never ? "" : ("\n- When mediating, dead " +
        "players will be able to see you"));

    public override void Init()
    {
        base.Init();
        MediatedPlayers.Clear();
        MediateArrows = [];
        Alignment = Alignment.CrewInvest;
        MediateButton ??= new(this, "MEDIATE", new SpriteName("Mediate"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Mediate, new Cooldown(MediateCd));
        // SeanceButton ??= new(this, "SEANCE", new SpriteName("Seance"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Seance, new Cooldown(SeanceCd), 1,
        //     new PostDeath(true));
    }

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

        if (!ShowMediatePlayer)
        {
            __instance.SetOutfit(CustomPlayerOutfitType.Camouflage, BlankOutfit(__instance));
            PlayerMaterial.SetColors(UColor.grey, __instance.MyRend());
        }
    }

    public void Mediate()
    {
        MediateButton.StartCooldown();
        var playersDead = KilledPlayers.GetRange(0, KilledPlayers.Count);

        if (playersDead.Count == 0)
            return;

        var bodies = AllBodies();

        if (DeadRevealed == DeadRevealed.Random)
        {
            var dead = playersDead.Random();

            if (bodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            {
                MediateArrows.Add(dead.PlayerId, new(Player, PlayerById(dead.PlayerId), Color, skipBody: true));
                MediatedPlayers.Add(dead.PlayerId);
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, dead.PlayerId);
            }
        }
        else
        {
            if (DeadRevealed == DeadRevealed.Newest)
                playersDead.Reverse();

            foreach (var dead in playersDead)
            {
                if (bodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                {
                    MediateArrows.Add(dead.PlayerId, new(Player, PlayerById(dead.PlayerId), Color, skipBody: true));
                    MediatedPlayers.Add(dead.PlayerId);
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, dead.PlayerId);

                    if (DeadRevealed != DeadRevealed.Everyone)
                        break;
                }
            }
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        var playerid2 = reader.ReadByte();
        MediatedPlayers.Add(playerid2);

        if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.Dead && ShowMediumToDead == ShowMediumToDead.AllDead))
            CustomPlayer.Local.GetRole().DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Player, Color, skipBody: true));
    }
}