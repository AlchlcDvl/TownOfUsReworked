namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Zealot)]
public sealed class Zealot : Neophyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number PreachCd = 25;

    [ToggleOption]
    private static bool ZealVent = false;

    [ToggleOption]
    public static bool FollowersVent = false;

    private CustomButton PreachButton;
    private bool HasConverted;

    protected override UColor MainColor => CustomColorManager.Zealot;
    public override Layer Type => Layer.Zealot;
    public override string StartText => "Brings Others To Your Religion At Any Cost";
    public override string Description => "- You can preach your gospel to a player in an attempt to convert them\n- If the target cannot be converted, you will kill them instead";
    public override Attack Attack => Attack.Basic;
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
            HasConverted = target.Is(Handler.CurrentFaction);
        }

        PreachButton.StartCooldown(cooldown);
    }

    private bool Usable() => !HasConverted;

    private bool Exception(PlayerControl player) => Members.Contains(player.PlayerId) || player.Is(Handler.CurrentFaction);
}