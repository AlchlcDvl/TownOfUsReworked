namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Monarch : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float KnightingCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RoundOneNoKnighting { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 14, 1)]
    public static int KnightCount { get; set; } = 2;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 10, 1)]
    public static int KnightVoteCount { get; set; } = 1;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MonarchButton { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool KnightButton { get; set; } = true;

    public bool RoundOne { get; set; }
    public CustomButton KnightingButton { get; set; }
    public List<byte> ToBeKnighted { get; set; }
    public List<byte> Knighted { get; set; }
    public bool Protected => Knighted.Any();

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Monarch : CustomColorManager.Crew;
    public override string Name => "Monarch";
    public override LayerEnum Type => LayerEnum.Monarch;
    public override Func<string> StartText => () => "Knight Those Who You Trust";
    public override Func<string> Description => () => $"- You can knight players\n- Knighted players will have their votes count {KnightVoteCount + 1} times\n- As long as a knight is alive,"
        + " you cannot be killed";
    public override DefenseEnum DefenseVal => Knighted.Any(x => !PlayerById(x).HasDied()) ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSov;
        Knighted = [];
        ToBeKnighted = [];
        KnightingButton = CreateButton(this, "KNIGHT", new SpriteName("Knight"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Knight, new Cooldown(KnightingCd), KnightCount,
            (PlayerBodyExclusion)Exception, (UsableFunc)Usable);
    }

    public void Knight()
    {
        var cooldown = Interact(Player, KnightingButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, KnightingButton.TargetPlayer.PlayerId);
            ToBeKnighted.Add(KnightingButton.TargetPlayer.PlayerId);
        }

        KnightingButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => ToBeKnighted.Contains(player.PlayerId) || player.IsKnighted();

    public override void ReadRPC(MessageReader reader) => ToBeKnighted.Add(reader.ReadByte());

    public bool Usable() => !RoundOne;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        var remove = new List<byte>();

        foreach (var id in Knighted)
        {
            var knight = PlayerById(id);

            if (knight.HasDied())
            {
                remove.Add(id);
                Run("<color=#FF004EFF>〖 Alert 〗</color>", "A Knight as died!");
            }
        }

        remove.ForEach(x => Knighted.Remove(x));
    }
}