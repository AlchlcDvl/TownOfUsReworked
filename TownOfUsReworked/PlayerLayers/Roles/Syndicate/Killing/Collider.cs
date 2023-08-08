namespace TownOfUsReworked.PlayerLayers.Roles;

public class Collider : Syndicate
{
    public CustomButton PositiveButton { get; set; }
    public CustomButton NegativeButton { get; set; }
    public CustomButton ChargeButton { get; set; }
    public DateTime LastPositive { get; set; }
    public DateTime LastNegative { get; set; }
    public DateTime LastCharged { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    public bool Enabled { get; set; }
    public float TimeRemaining { get; set; }
    public bool Charged => TimeRemaining > 0f;
    private float Range => CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Collider : Colors.Syndicate;
    public override string Name => "Collider";
    public override LayerEnum Type => LayerEnum.Collider;
    public override Func<string> StartText => () => "FUUUUUUUUUUUUUUUUUUUUUUUUUUSION!";
    public override Func<string> Description => () => $"- You can mark a player as positive or negative\n- When the marked players are within {Range}m of each other, they will die " +
        $"together{(HoldsDrive ? "\n- You can charge yourself to kill those you marked" : "")}\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.Unseen;
    public float PositiveTimer => ButtonUtils.Timer(Player, LastPositive, CustomGameOptions.CollideCooldown);
    public float NegativeTimer => ButtonUtils.Timer(Player, LastNegative, CustomGameOptions.CollideCooldown);
    public float ChargeTimer => ButtonUtils.Timer(Player, LastCharged, CustomGameOptions.ChargeCooldown);

    public Collider(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.SyndicateKill;
        Positive = null;
        Negative = null;
        PositiveButton = new(this, "Positive", AbilityTypes.Direct, "ActionSecondary", SetPositive, Exception1);
        NegativeButton = new(this, "Negative", AbilityTypes.Direct, "Secondary", SetNegative, Exception2);
        ChargeButton = new(this, "Charge", AbilityTypes.Effect, "Tertiary", Charge);
    }

    public void SetPositive()
    {
        if (IsTooFar(Player, PositiveButton.TargetPlayer) || PositiveTimer != 0f)
            return;

        var interact = Interact(Player, PositiveButton.TargetPlayer);

        if (interact[3])
            Positive = PositiveButton.TargetPlayer;

        if (interact[0])
            LastPositive = DateTime.UtcNow;
        else if (interact[1])
            LastPositive.AddSeconds(CustomGameOptions.ProtectKCReset);

        if (CustomGameOptions.ChargeCooldownsLinked)
        {
            if (interact[0])
                LastNegative = DateTime.UtcNow;
            else if (interact[1])
                LastNegative.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }

    public void SetNegative()
    {
        if (IsTooFar(Player, NegativeButton.TargetPlayer) || NegativeTimer != 0f)
            return;

        var interact = Interact(Player, NegativeButton.TargetPlayer);

        if (interact[3])
            Negative = NegativeButton.TargetPlayer;

        if (interact[0])
            LastNegative = DateTime.UtcNow;
        else if (interact[1])
            LastNegative.AddSeconds(CustomGameOptions.ProtectKCReset);

        if (CustomGameOptions.ChargeCooldownsLinked)
        {
            if (interact[0])
                LastPositive = DateTime.UtcNow;
            else if (interact[1])
                LastPositive.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }

    public void Charge()
    {
        if (!HoldsDrive || Charged || ChargeTimer != 0f)
            return;

        TimeRemaining = CustomGameOptions.ChargeDuration;
        ChargeSelf();
    }

    public void ChargeSelf()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (IsDead || Meeting)
            TimeRemaining = 0f;
    }

    public void DischargeSelf()
    {
        Enabled = false;
        LastCharged = DateTime.UtcNow;
    }

    public bool Exception1(PlayerControl player) => player == Negative || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public bool Exception2(PlayerControl player) => player == Positive || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        PositiveButton.Update("SET POSITIVE", PositiveTimer, CustomGameOptions.CollideCooldown);
        NegativeButton.Update("SET NEGATIVE", NegativeTimer, CustomGameOptions.CollideCooldown);
        ChargeButton.Update("CHARGE", ChargeTimer, CustomGameOptions.ChargeCooldown, Charged, TimeRemaining, CustomGameOptions.ChargeDuration, true, HoldsDrive);

        if (GetDistBetweenPlayers(Positive, Negative) <= Range)
        {
            if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(LayerEnum.Pestilence) || Negative.IsOnAlert() || Negative.IsVesting() ||
                Negative.IsRetShielded()))
            {
                RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
            }

            if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(LayerEnum.Pestilence) || Positive.IsOnAlert() || Positive.IsVesting() ||
                Positive.IsRetShielded()))
            {
                RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);
            }

            Positive = null;
            Negative = null;

            if (CustomGameOptions.CollideResetsCooldown)
            {
                LastPositive = DateTime.UtcNow;
                LastNegative = DateTime.UtcNow;
            }
        }
        else if (GetDistBetweenPlayers(Player, Negative) <= Range && HoldsDrive && Charged)
        {
            if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(LayerEnum.Pestilence) || Negative.IsOnAlert() || Negative.IsVesting() ||
                Negative.IsRetShielded()))
            {
                RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
            }

            Negative = null;

            if (CustomGameOptions.CollideResetsCooldown)
            {
                LastPositive = DateTime.UtcNow;
                LastNegative = DateTime.UtcNow;
            }
        }
        else if (GetDistBetweenPlayers(Player, Positive) <= Range && HoldsDrive && Charged)
        {
            if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(LayerEnum.Pestilence) || Positive.IsOnAlert() || Positive.IsVesting() ||
                Positive.IsRetShielded()))
            {
                RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);
            }

            Positive = null;

            if (CustomGameOptions.CollideResetsCooldown)
            {
                LastPositive = DateTime.UtcNow;
                LastNegative = DateTime.UtcNow;
            }
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        Positive = null;
        Negative = null;
    }
}