namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Dracula : Neophyte
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
    public static int AliveCount => AllPlayers().Count(x => !x.HasDied());

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Dracula : FactionColor;
    public override LayerEnum Type => LayerEnum.Dracula;
    public override Func<string> StartText => () => "Lead The <#7B8968FF>Undead</color> To Victory";
    public override Func<string> Description => () => "- You can convert the <#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the number of alive" +
        $" <#7B8968FF>Undead</color> exceeds {AliveVampCount}, you will kill them instead";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Convert or kill anyone who can oppose the <#7B8968FF>Undead</color>";
        SubFaction = SubFaction.Undead;
        BiteButton ??= new(this, new SpriteName("Bite"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Convert, new Cooldown(BiteCd), "BITE", (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
    }

    public override void OnMeetingEnd(MeetingHud __instance)
    {
        base.OnMeetingEnd(__instance);
        HasConverted = false;
    }

    public void Convert(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            RpcConvert(target.PlayerId, Player.PlayerId, SubFaction.Undead, AliveCount >= AliveVampCount);
            HasConverted = true;
        }

        BiteButton.StartCooldown(cooldown);
    }

    public bool Usable() => !HasConverted;

    public bool Exception(PlayerControl player) => Members.Contains(player.PlayerId);
}