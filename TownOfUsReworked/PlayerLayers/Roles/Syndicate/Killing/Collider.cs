namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Collider)]
public sealed class Collider : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number CollideCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ChargeCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number ChargeDur = 10;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number CollideRange = 1.5f;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number CollideRangeIncrease = 0.5f;

    [ToggleOption]
    public static bool ChargeCooldownsLinked = false;

    [ToggleOption]
    public static bool CollideResetsCooldown = false;

    private CustomButton PositiveButton { get; set; }
    private CustomButton NegativeButton { get; set; }
    private CustomButton ChargeButton { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    private float Range => CollideRange + (HoldsDrive ? CollideRangeIncrease : 0);

    protected override UColor MainColor => CustomColorManager.Collider;
    public override LayerEnum Type => LayerEnum.Collider;
    public override Func<string> StartText { get; } = () => "FUUUUUUUUUUUUUUUUUUUUUUUUUUSION!";
    public override Func<string> Description => () => $"- You can mark a player as positive or negative\n- When the marked players are within {Range}m of each other, they will die together" +
        $"{(HoldsDrive ? "\n- You can charge yourself to kill those you marked" : "")}\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
        Positive = null;
        Negative = null;
        PositiveButton ??= new(this, new SpriteName("Positive"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SetPositive, new Cooldown(CollideCd), "SET POSITIVE",
            (PlayerBodyExclusion)Exception1);
        NegativeButton ??= new(this, new SpriteName("Negative"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)SetNegative, new Cooldown(CollideCd), "SET NEGATIVE",
            (PlayerBodyExclusion)Exception2);
        ChargeButton ??= new(this, new SpriteName("Charge"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Charge, new Cooldown(ChargeCd), "CHARGE", (UsableFunc)Usable,
            new Duration(ChargeDur), (EndFunc)EndEffect);
    }

    public override void Reset(bool meeting, bool start) => Positive = Negative = null;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Positive == player)
            name += " <#B345FFFF>+</color>";

        if (Negative == player)
            name += " <#B345FFFF>-</color>";
    }

    private void Charge() => ChargeButton.Begin();

    protected override void Deinit()
    {
        Positive = null;
        Negative = null;
    }

    private void SetPositive(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Positive = target;

        PositiveButton.StartCooldown(cooldown);

        if (ChargeCooldownsLinked)
            NegativeButton.StartCooldown(cooldown);
    }

    private void SetNegative(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Negative = target;

        NegativeButton.StartCooldown(cooldown);

        if (ChargeCooldownsLinked)
            PositiveButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => player == Negative || CommonException(player);

    private bool Exception2(PlayerControl player) => player == Positive || CommonException(player);

    private bool CommonException(PlayerControl player) => Player.IsBuddyWith(player, Faction);

    private bool Usable() => HoldsDrive;

    public override void UpdateHud(HudManager __instance)
    {
        if (Dead)
            return;

        var shouldReset = false;

        if (GetDistance(Positive, Negative) <= Range)
        {
            if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                Player.RpcMurderPlayer(Negative, DeathReasonEnum.Collided, false);

            if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                Player.RpcMurderPlayer(Positive, DeathReasonEnum.Collided, false);

            Positive = null;
            Negative = null;
            shouldReset = true;
        }
        else if (GetDistance(Player, Negative) <= Range && HoldsDrive && ChargeButton.EffectActive)
        {
            if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                Player.RpcMurderPlayer(Negative, DeathReasonEnum.Collided, false);

            Negative = null;
            shouldReset = true;
        }
        else if (GetDistance(Player, Positive) <= Range && HoldsDrive && ChargeButton.EffectActive)
        {
            if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                Player.RpcMurderPlayer(Positive, DeathReasonEnum.Collided, false);

            Positive = null;
            shouldReset = true;
        }

        if (CollideResetsCooldown && shouldReset)
        {
            PositiveButton.StartCooldown();
            NegativeButton.StartCooldown();
        }
    }

    private bool EndEffect() => Dead;

    public override void BeforeMeeting() => Deinit();
}