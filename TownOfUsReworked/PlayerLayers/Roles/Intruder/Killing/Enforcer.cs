namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Enforcer : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number EnforceCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number EnforceDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 15f, 1f, Format.Time)]
    public static Number EnforceDelay { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static Number EnforceRadius { get; set; } = new(1.5f);

    public CustomButton BombButton { get; set; }
    public PlayerControl BombedPlayer { get; set; }
    public bool BombSuccessful { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Enforcer : CustomColorManager.Intruder;
    public override string Name => "Enforcer";
    public override LayerEnum Type => LayerEnum.Enforcer;
    public override Func<string> StartText => () => "Force The <color=#8CFFFFFF>Crew</color> To Do Your Bidding";
    public override Func<string> Description => () => "- You can plant bombs on players and force them to kill others\n- If the player is unable to kill someone within " +
        $"{EnforceDur}s, the bomb will detonate and kill everyone within a {EnforceRadius}m radius\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.IntruderKill;
        BombedPlayer = null;
        BombButton ??= new(this, new SpriteName("Enforce"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Bomb, new Cooldown(EnforceCd), "SET BOMB", new Duration(EnforceDur),
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
                RpcMurderPlayer(enf, player, DeathReasonEnum.Bombed, false);
        }
    }

    public void Bomb()
    {
        var target = BombButton.GetTarget<PlayerControl>();
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