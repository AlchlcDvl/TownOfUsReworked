namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Monarch : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number KnightingCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RoundOneNoKnighting { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number KnightCount { get; set; } = new(2);

    [NumberOption(MultiMenu.LayerSubOptions, 1, 10, 1)]
    public static Number KnightVoteCount { get; set; } = new(1);

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
        base.Init();
        Alignment = Alignment.CrewSov;
        Knighted = [];
        ToBeKnighted = [];
        KnightingButton ??= new(this, "KNIGHT", new SpriteName("Knight"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Knight, new Cooldown(KnightingCd), KnightCount,
            (PlayerBodyExclusion)Exception, (UsableFunc)Usable);
    }

    public void Knight()
    {
        var target = KnightingButton.GetTarget<PlayerControl>();
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);
            ToBeKnighted.Add(target.PlayerId);
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