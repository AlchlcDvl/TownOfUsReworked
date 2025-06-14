namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Tracker)]
public sealed class Tracker : Investigative
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxTracks = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TrackCd = 25;

    [ToggleOption]
    public static bool ResetOnNewRound = false;

    [NumberOption(0f, 15f, 0.5f, Format.Time)]
    public static Number UpdateInterval = 5;

    private readonly Dictionary<byte, PlayerArrow> TrackerArrows = [];
    private CustomButton TrackButton;

    protected override UColor MainColor => CustomColorManager.Tracker;
    public override LayerEnum Type => LayerEnum.Tracker;
    public override string StartText => "Track Everyone's Movements";
    public override string Description => "- You can track players which creates arrows that update every now and then with the target's position";

    public override void Init()
    {
        base.Init();
        TrackerArrows.Clear();
        TrackButton ??= new(this, "TRACK", new SpriteName("Track"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Track, new Cooldown(TrackCd), MaxTracks,
            (PlayerBodyExclusion)Exception);
    }

    private bool Exception(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

    private void Track(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            TrackerArrows.Add(target.PlayerId, new(Player, target, target.GetPlayerColor(), UpdateInterval));

        TrackButton.StartCooldown(cooldown);
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer) => ClearArrows();

    protected override void ClearArrows()
    {
        TrackerArrows.Values.DestroyAll();
        TrackerArrows.Clear();
    }

    public override void Reset(bool meeting, bool start)
    {
        if (!ResetOnNewRound)
            return;

        TrackButton.Uses = TrackButton.MaxUses;
        ClearArrows();
    }
}