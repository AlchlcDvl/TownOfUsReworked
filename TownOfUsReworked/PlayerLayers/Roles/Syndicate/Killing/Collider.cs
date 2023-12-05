namespace TownOfUsReworked.PlayerLayers.Roles;

public class Collider : Syndicate
{
    public CustomButton PositiveButton { get; set; }
    public CustomButton NegativeButton { get; set; }
    public CustomButton ChargeButton { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    private float Range => CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Collider : Colors.Syndicate;
    public override string Name => "Collider";
    public override LayerEnum Type => LayerEnum.Collider;
    public override Func<string> StartText => () => "FUUUUUUUUUUUUUUUUUUUUUUUUUUSION!";
    public override Func<string> Description => () => $"- You can mark a player as positive or negative\n- When the marked players are within {Range}m of each other, they will die together" +
        $"{(HoldsDrive ? "\n- You can charge yourself to kill those you marked" : "")}\n{CommonAbilities}";

    public Collider(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateKill;
        Positive = null;
        Negative = null;
        PositiveButton = new(this, "Positive", AbilityTypes.Target, "ActionSecondary", SetPositive, CustomGameOptions.CollideCd, Exception1);
        NegativeButton = new(this, "Negative", AbilityTypes.Target, "Secondary", SetNegative, CustomGameOptions.CollideCd, Exception2);
        ChargeButton = new(this, "Charge", AbilityTypes.Targetless, "Tertiary", Charge, CustomGameOptions.ChargeCd, CustomGameOptions.ChargeDur);
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
        var interact = Interact(Player, PositiveButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Positive = PositiveButton.TargetPlayer;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        PositiveButton.StartCooldown(cooldown);

        if (CustomGameOptions.ChargeCooldownsLinked)
            NegativeButton.StartCooldown(cooldown);
    }

    public void SetNegative()
    {
        var interact = Interact(Player, NegativeButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Negative = NegativeButton.TargetPlayer;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        NegativeButton.StartCooldown(cooldown);

        if (CustomGameOptions.ChargeCooldownsLinked)
            PositiveButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == Negative || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || Player.IsLinkedTo(player);

    public bool Exception2(PlayerControl player) => player == Positive || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        PositiveButton.Update2("SET POSITIVE");
        NegativeButton.Update2("SET NEGATIVE");
        ChargeButton.Update2("CHARGE", HoldsDrive);

        if (GetDistance(Positive, Negative) <= Range)
        {
            if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(LayerEnum.Pestilence) || Negative.IsOnAlert() || Negative.IsVesting()))
                RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);

            if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(LayerEnum.Pestilence) || Positive.IsOnAlert() || Positive.IsVesting()))
                RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);

            Positive = null;
            Negative = null;

            if (CustomGameOptions.CollideResetsCooldown)
            {
                PositiveButton.StartCooldown();
                NegativeButton.StartCooldown();
            }
        }
        else if (GetDistance(Player, Negative) <= Range && HoldsDrive && ChargeButton.EffectActive)
        {
            if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(LayerEnum.Pestilence) || Negative.IsOnAlert() || Negative.IsVesting()))
                RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);

            Negative = null;

            if (CustomGameOptions.CollideResetsCooldown)
            {
                PositiveButton.StartCooldown();
                NegativeButton.StartCooldown();
            }
        }
        else if (GetDistance(Player, Positive) <= Range && HoldsDrive && ChargeButton.EffectActive)
        {
            if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(LayerEnum.Pestilence) || Positive.IsOnAlert() || Positive.IsVesting()))
                RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);

            Positive = null;

            if (CustomGameOptions.CollideResetsCooldown)
            {
                PositiveButton.StartCooldown();
                NegativeButton.StartCooldown();
            }
        }
    }

    public override void TryEndEffect() => ChargeButton.Update3(IsDead);

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        ResetCharges();
    }
}