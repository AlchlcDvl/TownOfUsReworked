namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Enforcer : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number EnforceCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number EnforceDur = 10;

    [NumberOption(1f, 15f, 1f, Format.Time)]
    public static Number EnforceDelay = 5;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number EnforceRadius = 1.5f;

    public CustomButton BombButton { get; set; }
    public PlayerControl BombedPlayer { get; set; }
    public bool BombSuccessful { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Enforcer : FactionColor;
    public override LayerEnum Type => LayerEnum.Enforcer;
    public override Func<string> StartText => () => "Force The <#8CFFFFFF>Crew</color> To Do Your Bidding";
    public override Func<string> Description => () => "- You can plant bombs on players and force them to kill others\n- If the player is unable to kill someone within " +
        $"{EnforceDur}s, the bomb will detonate and kill everyone within a {EnforceRadius}m radius\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
        BombedPlayer = null;
        BombButton ??= new(this, new SpriteName("Enforce"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Bomb, new Cooldown(EnforceCd), "SET BOMB", new Duration(EnforceDur),
            (EffectStartVoid)BoomStart, (EffectStartVoid)UnBoom, new Delay(EnforceDelay), (PlayerBodyExclusion)Exception1, new CanClickAgain(false), (EndFunc)EndEffect);
    }

    public void BoomStart()
    {
        if (BombedPlayer.AmOwner && !Dead)
        {
            Flash(Color);
            BombedPlayer.GetRole().Bombed = true;
        }
    }

    public void UnBoom()
    {
        if (!BombSuccessful)
            Explode(BombedPlayer, Player);

        BombedPlayer.GetRole().Bombed = false;
        BombedPlayer = null;
        BombSuccessful = false;
    }

    public static void Explode(PlayerControl centre, PlayerControl enf)
    {
        foreach (var player in GetClosestPlayers(centre, EnforceRadius))
        {
            if (CanAttack(AttackEnum.Powerful, player.GetDefenseValue()))
                enf.RpcMurderPlayer(player, DeathReasonEnum.Bombed, false);
        }

        enf.RpcMurderPlayer(centre, DeathReasonEnum.Bombed, false);
    }

    public void Bomb(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BombedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BombButton, BombedPlayer);
            BombButton.Begin();
        }
        else
            BombButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == BombedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction
        != SubFaction.None) || Player.IsLinkedTo(player);

    public bool EndEffect() => (BombedPlayer && BombedPlayer.HasDied()) || Dead || BombSuccessful;

    public override void ReadRPC(MessageReader reader) => BombedPlayer = reader.ReadPlayer();
}