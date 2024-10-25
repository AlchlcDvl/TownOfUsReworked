namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Dracula : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number BiteCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool DracVent { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 14, 1)]
    public static Number AliveVampCount { get; set; } = new(3);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool UndeadVent { get; set; } = false;

    public CustomButton BiteButton { get; set; }
    public bool HasConverted { get; set; }
    public List<byte> Converted { get; set; }
    public static int AliveCount => AllPlayers().Count(x => !x.HasDied());

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Dracula : CustomColorManager.Neutral;
    public override string Name => "Dracula";
    public override LayerEnum Type => LayerEnum.Dracula;
    public override Func<string> StartText => () => "Lead The <color=#7B8968FF>Undead</color> To Victory";
    public override Func<string> Description => () => "- You can convert the <color=#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the number of " +
        $"alive <color=#7B8968FF>Undead</color> exceeds {AliveVampCount}, you will kill them instead\n- Attempting to convert a <color=#C0C0C0FF>Vampire Hunter</color> will force them to " +
        "kill you";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Convert or kill anyone who can oppose the <color=#7B8968FF>Undead</color>";
        SubFaction = SubFaction.Undead;
        Alignment = Alignment.NeutralNeo;
        SubFactionColor = CustomColorManager.Undead;
        Converted = [ Player.PlayerId ];
        BiteButton ??= CreateButton(this, new SpriteName("Bite"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Convert, new Cooldown(BiteCd), "BITE",
            (PlayerBodyExclusion)Exception, (UsableFunc)Usable);
    }

    public override void OnMeetingEnd(MeetingHud __instance)
    {
        base.OnMeetingEnd(__instance);
        HasConverted = false;
    }

    public void Convert()
    {
        var cooldown = Interact(Player, BiteButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
        {
            RoleGen.RpcConvert(BiteButton.GetTarget<PlayerControl>().PlayerId, Player.PlayerId, SubFaction.Undead, AliveCount >= AliveVampCount);
            HasConverted = true;
        }

        BiteButton.StartCooldown(cooldown);
    }

    public bool Usable() => !HasConverted;

    public bool Exception(PlayerControl player) => Converted.Contains(player.PlayerId);
}