namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Monarch : Crew, ISovereign
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number KnightingCd = 25;

    [ToggleOption]
    public static bool RoundOneNoKnighting = false;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number KnightCount = 2;

    [NumberOption(1, 10, 1)]
    public static Number KnightVoteCount = 1;

    [ToggleOption]
    public static bool MonarchButton = true;

    [ToggleOption]
    public static bool KnightButton = true;

    public bool RoundOne { get; set; }
    public CustomButton KnightingButton { get; set; }
    public List<byte> ToBeKnighted { get; } = [];
    public List<byte> Knighted { get; } = [];

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Monarch : FactionColor;
    public override LayerEnum Type => LayerEnum.Monarch;
    public override Func<string> StartText => () => "Knight Those Who You Trust";
    public override Func<string> Description => () => $"- You can knight players\n- Knighted players will have their votes count {KnightVoteCount + 1} times\n- As long as a knight is alive,"
        + " you cannot be killed\n- You will know when a knight of yours dies";
    public override DefenseEnum DefenseVal => Knighted.Any(x => !PlayerById(x).HasDied()) ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Sovereign;
        Knighted.Clear();
        ToBeKnighted.Clear();
        KnightingButton ??= new(this, "KNIGHT", new SpriteName("Knight"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Knight, new Cooldown(KnightingCd), KnightCount,
            (PlayerBodyExclusion)Exception, (UsableFunc)Usable);
    }

    public void Knight(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            ToBeKnighted.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);

            if (target.TryGetILayer<IRevealer>(out var rev) && !rev.Revealed)
                CustomAchievementManager.UnlockAchievement("HiddenAlliance");
        }

        KnightingButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => ToBeKnighted.Contains(player.PlayerId) || player.IsKnighted();

    public override void ReadRPC(MessageReader reader)
    {
        var id = reader.ReadByte();
        ToBeKnighted.Add(id);

        if (CustomPlayer.Local.PlayerId == id && CustomPlayer.Local.TryGetILayer<IRevealer>(out var rev) && !rev.Revealed)
            CustomAchievementManager.UnlockAchievement("HiddenAlliance");
    }

    public bool Usable() => !RoundOne;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        Knighted.RemoveAll(x => PlayerById(x).HasDied());
    }
}