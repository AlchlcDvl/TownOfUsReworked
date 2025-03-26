namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Stalker)]
public sealed class Stalker : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number StalkCd = 25;

    private Dictionary<byte, PlayerArrow> StalkerArrows { get; } = [];
    private CustomButton StalkButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Stalker;
    public override LayerEnum Type => LayerEnum.Stalker;
    public override Func<string> StartText => () => "Stalk Everyone To Monitor Their Movements";
    public override Func<string> Description => () => "- You always know where your targets are" + (HoldsDrive ? "\n- Camouflages do not stop you seeing who's where" : "") + "\n" +
        CommonAbilities;

    protected override void Init()
    {
        base.Init();
        StalkerArrows.Clear();
        Alignment = Alignment.Support;
        StalkButton ??= new(this, new SpriteName("Stalk"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Stalk, new Cooldown(StalkCd), "STALK", (UsableFunc)Usable,
            (PlayerBodyExclusion)Exception1);
    }

    public override void ClearArrows()
    {
        base.ClearArrows();
        StalkerArrows.Values.DestroyAll();
        StalkerArrows.Clear();
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        base.OnDeath(reason, reason2, killer);
        ClearArrows();
    }

    private void Stalk(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            StalkerArrows.Add(target.PlayerId, new(Player, target, target.GetPlayerColor(!HoldsDrive, false, !HoldsDrive)));

        StalkButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    private bool Usable() => !HoldsDrive;

    protected override void OnDriveReceivedLocal()
    {
        foreach (var player in AllPlayers())
        {
            if (!StalkerArrows.ContainsKey(player.PlayerId))
                StalkerArrows.Add(player.PlayerId, new(Player, player, player.GetPlayerColor(false, false, false)));
        }
    }
}