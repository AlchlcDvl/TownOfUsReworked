namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Zealot)]
public sealed class Zealot : Neophyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number PreachCd = 25;

    [ToggleOption]
    private static bool ZealVent = false;

    [ToggleOption]
    public static bool FollowersVent = false;

    private CustomButton PreachButton { get; set; }
    private bool HasConverted { get; set; }

    protected override UColor MainColor => CustomColorManager.Zealot;
    public override LayerEnum Type => LayerEnum.Zealot;
    public override Func<string> StartText { get; } = () => "Brings Others To Your Religion At Any Cost";
    public override Func<string> Description => () => "- You can preach your gospel to a player in an attempt to convert them\n- If the target cannot be converted, you will kill them instead";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override bool CanVent => base.CanVent && ZealVent;
    protected override Faction ActualFaction => Faction.Followers;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Convert or kill anyone who can oppose the <#917AC0FF>Followers</color>";
        PreachButton ??= new(this, new SpriteName("Preach"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Convert, new Cooldown(PreachCd), "PREACH", (UsableFunc)Usable,
            (PlayerBodyExclusion)Exception);
    }

    private void Convert(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            RpcConvert(target.PlayerId, Player.PlayerId);
            HasConverted = target.Is(Faction);
        }

        PreachButton.StartCooldown(cooldown);
    }

    private bool Usable() => !HasConverted;

    private bool Exception(PlayerControl player) => Members.Contains(player.PlayerId) || player.Is(Faction);
}