namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Stalker : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number StalkCd { get; set; } = new(25);

    public Dictionary<byte, PlayerArrow> StalkerArrows { get; set; }
    public CustomButton StalkButton { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Stalker : FactionColor;
    public override string Name => "Stalker";
    public override LayerEnum Type => LayerEnum.Stalker;
    public override Func<string> StartText => () => "Stalk Everyone To Monitor Their Movements";
    public override Func<string> Description => () => $"- You always know where your targets are" + (HoldsDrive ? "\n- Camouflages do not stop you seeing who's where" : "") + "\n" +
        CommonAbilities;

    public override void Init()
    {
        base.Init();
        StalkerArrows = [];
        Alignment = Alignment.SyndicateSupport;
        StalkButton ??= new(this, new SpriteName("Stalk"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Stalk, new Cooldown(StalkCd), "STALK", (UsableFunc)Usable,
            (PlayerBodyExclusion)Exception1);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        StalkerArrows.Remove(targetPlayerId);
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

    public void Stalk(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            StalkerArrows.Add(target.PlayerId, new(Player, target, target.GetPlayerColor(!HoldsDrive, false, !HoldsDrive)));

        StalkButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    public bool Usable() => !HoldsDrive;

    public override void OnDriveReceivedLocal()
    {
        foreach (var player in AllPlayers())
        {
            if (!StalkerArrows.ContainsKey(player.PlayerId))
                StalkerArrows.Add(player.PlayerId, new(Player, player, player.GetPlayerColor(false, false, false)));
        }
    }
}