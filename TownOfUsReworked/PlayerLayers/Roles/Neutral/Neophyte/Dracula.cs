namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Dracula : Neophyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number BiteCd = 25;

    [ToggleOption]
    public static bool DracVent = false;

    [NumberOption(1, 14, 1)]
    private static Number AliveVampCount = 3;

    [ToggleOption]
    public static bool UndeadVent = false;

    private static int AliveCount => AllPlayers().Count(x => !x.HasDied());

    private CustomButton BiteButton { get; set; }
    private bool HasConverted { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Dracula : FactionColor;
    public override LayerEnum Type => LayerEnum.Dracula;
    public override Func<string> StartText => () => "Lead The <#7B8968FF>Undead</color> To Victory";
    public override Func<string> Description => () => "- You can convert the <#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the number of alive" +
        $" <#7B8968FF>Undead</color> exceeds {AliveVampCount}, you will kill them instead";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    protected override void Init()
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

    private void Convert(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            RpcConvert(target.PlayerId, Player.PlayerId, SubFaction.Undead, AliveCount >= AliveVampCount);
            HasConverted = true;
        }

        BiteButton.StartCooldown(cooldown);
    }

    private bool Usable() => !HasConverted;

    private bool Exception(PlayerControl player) => Members.Contains(player.PlayerId);
}