namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Monarch)]
public sealed class Monarch : Sovereign
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number KnightingCd = 25;

    [ToggleOption]
    private static bool RoundOneNoKnighting = false;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    private static Number KnightCount = 2;

    [NumberOption(1, 10, 1)]
    public static Number KnightVoteCount = 1;

    [ToggleOption]
    public static bool MonarchButton = true;

    [ToggleOption]
    public static bool KnightButton = true;

    private bool RoundOne;
    private CustomButton KnightingButton;

    public readonly HashSet<byte> ToBeKnighted = [];
    public readonly HashSet<byte> Knighted = [];

    protected override UColor MainColor => CustomColorManager.Monarch;
    public override Layer Type => Layer.Monarch;
    public override string StartText => "Knight Those Who You Trust";
    public override string Description => $"- You can knight players\n- Knighted players will have their votes count {KnightVoteCount + 1} times\n- As long as a knight is alive,"
        + " you cannot be killed\n- You will know when a knight of yours dies";
    public override Defense Defense => Knighted.Any(x => !PlayerById(x).HasDied()) ? Defense.Basic : Defense.None;

    public override void Init()
    {
        Knighted.Clear();
        ToBeKnighted.Clear();
        KnightingButton ??= new(this, "KNIGHT", new SpriteName("Knight"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Knight, new Cooldown(KnightingCd), KnightCount,
            (PlayerBodyExclusion)Exception, (UsableFunc)Usable);
    }

    public override void Reset(bool meeting, bool start) => RoundOne = start && RoundOneNoKnighting;

    private void Knight(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            ToBeKnighted.Add(target.PlayerId);
            PerformRpcAction(target.PlayerId);

            if (target.Is<Sovereign>(out var rev) && !rev.Revealed)
                CustomAchievementManager.UnlockAchievement("HiddenAlliance");
        }

        KnightingButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => ToBeKnighted.Contains(player.PlayerId) || player.IsKnighted();

    public override void ReadRPC(RpcReader reader)
    {
        var id = reader.ReadByte();
        ToBeKnighted.Add(id);

        if (LocalPlayer.PlayerId == id && LocalPlayer.Is<Sovereign>(out var rev) && !rev.Revealed)
            CustomAchievementManager.UnlockAchievement("HiddenAlliance");
    }

    private bool Usable() => !RoundOne;

    public override void LocalOnMeetingStart(MeetingHud __instance) => Knighted.RemoveAll(x => PlayerById(x).HasDied());
}