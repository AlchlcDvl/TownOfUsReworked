namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Tracker : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, zeroIsInf: true)]
    public static Number MaxTracks { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number TrackCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ResetOnNewRound { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 0.5f, Format.Time)]
    public static Number UpdateInterval { get; set; } = new(5);

    public Dictionary<byte, PlayerArrow> TrackerArrows { get; } = [];
    public CustomButton TrackButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Tracker : FactionColor;
    public override LayerEnum Type => LayerEnum.Tracker;
    public override Func<string> StartText => () => "Track Everyone's Movements";
    public override Func<string> Description => () => "- You can track players which creates arrows that update every now and then with the target's position";

    public override void Init()
    {
        base.Init();
        TrackerArrows.Clear();
        Alignment = Alignment.CrewInvest;
        TrackButton ??= new(this, "TRACK", new SpriteName("Track"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Track, new Cooldown(TrackCd), MaxTracks,
            (PlayerBodyExclusion)Exception);
    }

    public bool Exception(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

    public void Track(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            TrackerArrows.Add(target.PlayerId, new(Player, target, target.GetPlayerColor(), UpdateInterval));

        TrackButton.StartCooldown(cooldown);
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        base.OnDeath(reason, reason2, killer);
        ClearArrows();
    }

    public override void ClearArrows()
    {
        base.ClearArrows();
        TrackerArrows.Values.DestroyAll();
        TrackerArrows.Clear();
    }
}