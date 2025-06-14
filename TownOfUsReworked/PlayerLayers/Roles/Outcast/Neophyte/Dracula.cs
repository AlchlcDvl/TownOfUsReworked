namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Dracula)]
public sealed class Dracula : Neophyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number BiteCd = 25;

    [ToggleOption]
    private static bool DracVent = false;

    [NumberOption(1, 14, 1)]
    private static Number AliveVampCount = 3;

    [ToggleOption]
    public static bool UndeadVent = false;

    private static int AliveCount => AllPlayers().Count(x => !x.HasDied());

    private CustomButton BiteButton;
    private bool HasConverted;

    protected override UColor MainColor => CustomColorManager.Dracula;
    public override LayerEnum Type => LayerEnum.Dracula;
    public override string StartText => "Lead The <#7B8968FF>Undead</color> To Victory";
    public override string Description => "- You can bite a player in an attempt to convert them\n- If the target cannot be converted or the number of alive <#7B8968FF>Undead</color> " +
        $"exceeds {AliveVampCount}, you will kill them instead";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override bool CanVent => base.CanVent && DracVent;
    protected override Faction ActualFaction => Faction.Undead;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Convert or kill anyone who can oppose the <#7B8968FF>Undead</color>";
        BiteButton ??= new(this, new SpriteName("Bite"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Convert, new Cooldown(BiteCd), "BITE", (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
    }

    public override void OnMeetingEnd(MeetingHud __instance) => HasConverted = false;

    private void Convert(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            RpcConvert(target.PlayerId, Player.PlayerId, AliveCount >= AliveVampCount);
            HasConverted = true;
        }

        BiteButton.StartCooldown(cooldown);
    }

    private bool Usable() => !HasConverted;

    private bool Exception(PlayerControl player) => Members.Contains(player.PlayerId);
}