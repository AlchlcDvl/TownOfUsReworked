namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Collider : Syndicate
{
    public CustomButton PositiveButton { get; set; }
    public CustomButton NegativeButton { get; set; }
    public CustomButton ChargeButton { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    private float Range => CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Collider : CustomColorManager.Syndicate;
    public override string Name => "Collider";
    public override LayerEnum Type => LayerEnum.Collider;
    public override Func<string> StartText => () => "FUUUUUUUUUUUUUUUUUUUUUUUUUUSION!";
    public override Func<string> Description => () => $"- You can mark a player as positive or negative\n- When the marked players are within {Range}m of each other, they will die together" +
        $"{(HoldsDrive ? "\n- You can charge yourself to kill those you marked" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateKill;
        Positive = null;
        Negative = null;
        PositiveButton = CreateButton(this, new SpriteName("Positive"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)SetPositive, new Cooldown(CustomGameOptions.CollideCd),
            (PlayerBodyExclusion)Exception1, "SET POSITIVE");
        NegativeButton = CreateButton(this, new SpriteName("Negative"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)SetNegative, new Cooldown(CustomGameOptions.CollideCd),
            (PlayerBodyExclusion)Exception2, "SET NEGATIVE");
        ChargeButton = CreateButton(this, new SpriteName("Charge"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClick)Charge, new Cooldown(CustomGameOptions.ChargeCd), "CHARGE",
            new Duration(CustomGameOptions.ChargeDur), (UsableFunc)Usable, (EndFunc)EndEffect);
    }

    public void Charge() => ChargeButton.Begin();

    public void ResetCharges()
    {
        Positive = null;
        Negative = null;
    }

    public override void OnLobby()
    {
        base.OnLobby();
        ResetCharges();
    }

    public void SetPositive()
    {
        var cooldown = Interact(Player, PositiveButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            Positive = PositiveButton.TargetPlayer;

        PositiveButton.StartCooldown(cooldown);

        if (CustomGameOptions.ChargeCooldownsLinked)
            NegativeButton.StartCooldown(cooldown);
    }

    public void SetNegative()
    {
        var cooldown = Interact(Player, NegativeButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            Negative = NegativeButton.TargetPlayer;

        NegativeButton.StartCooldown(cooldown);

        if (CustomGameOptions.ChargeCooldownsLinked)
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

        if (CustomGameOptions.CollideResetsCooldown && shouldReset)
        {
            PositiveButton.StartCooldown();
            NegativeButton.StartCooldown();
        }
    }

    public bool EndEffect() => Dead;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        ResetCharges();
    }
}