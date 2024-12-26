namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Collider : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number CollideCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ChargeCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number ChargeDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static Number CollideRange { get; set; } = new(1.5f);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static Number CollideRangeIncrease { get; set; } = new(0.5f);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ChargeCooldownsLinked { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CollideResetsCooldown { get; set; } = false;

    public CustomButton PositiveButton { get; set; }
    public CustomButton NegativeButton { get; set; }
    public CustomButton ChargeButton { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    private float Range => CollideRange + (HoldsDrive ? CollideRangeIncrease : 0);

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Collider : FactionColor;
    public override string Name => "Collider";
    public override LayerEnum Type => LayerEnum.Collider;
    public override Func<string> StartText => () => "FUUUUUUUUUUUUUUUUUUUUUUUUUUSION!";
    public override Func<string> Description => () => $"- You can mark a player as positive or negative\n- When the marked players are within {Range}m of each other, they will die together" +
        $"{(HoldsDrive ? "\n- You can charge yourself to kill those you marked" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicateKill;
        Positive = null;
        Negative = null;
        PositiveButton ??= new(this, new SpriteName("Positive"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SetPositive, new Cooldown(CollideCd), "SET POSITIVE",
            (PlayerBodyExclusion)Exception1);
        NegativeButton ??= new(this, new SpriteName("Negative"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)SetNegative, new Cooldown(CollideCd), "SET NEGATIVE",
            (PlayerBodyExclusion)Exception2);
        ChargeButton ??= new(this, new SpriteName("Charge"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Charge, new Cooldown(ChargeCd), "CHARGE", (UsableFunc)Usable,
            new Duration(ChargeDur), (EndFunc)EndEffect);
    }

    public void Charge() => ChargeButton.Begin();

    public void ResetCharges()
    {
        Positive = null;
        Negative = null;
    }

    public override void Deinit()
    {
        base.Deinit();
        ResetCharges();
    }

    public void SetPositive(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Positive = target;

        PositiveButton.StartCooldown(cooldown);

        if (ChargeCooldownsLinked)
            NegativeButton.StartCooldown(cooldown);
    }

    public void SetNegative(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Negative = target;

        NegativeButton.StartCooldown(cooldown);

        if (ChargeCooldownsLinked)
            PositiveButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == Negative || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || Player.IsLinkedTo(player);

    public bool Exception2(PlayerControl player) => player == Positive || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || Player.IsLinkedTo(player);

    public bool Usable() => HoldsDrive;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Dead)
            return;

        var shouldReset = false;

        if (GetDistance(Positive, Negative) <= Range)
        {
            if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);

            if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);

            Positive = null;
            Negative = null;
            shouldReset = true;
        }
        else if (GetDistance(Player, Negative) <= Range && HoldsDrive && ChargeButton.EffectActive)
        {
            if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);

            Negative = null;
            shouldReset = true;
        }
        else if (GetDistance(Player, Positive) <= Range && HoldsDrive && ChargeButton.EffectActive)
        {
            if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);

            Positive = null;
            shouldReset = true;
        }

        if (CollideResetsCooldown && shouldReset)
        {
            PositiveButton.StartCooldown();
            NegativeButton.StartCooldown();
        }
    }

    public bool EndEffect() => Dead;

    public override void BeforeMeeting() => ResetCharges();
}