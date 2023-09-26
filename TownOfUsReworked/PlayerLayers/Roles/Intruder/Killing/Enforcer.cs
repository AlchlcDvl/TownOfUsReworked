namespace TownOfUsReworked.PlayerLayers.Roles;

public class Enforcer : Intruder
{
    public CustomButton BombButton { get; set; }
    public PlayerControl BombedPlayer { get; set; }
    public bool BombSuccessful { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Enforcer : Colors.Intruder;
    public override string Name => "Enforcer";
    public override LayerEnum Type => LayerEnum.Enforcer;
    public override Func<string> StartText => () => "Force The <color=#8CFFFFFF>Crew</color> To Do Your Bidding";
    public override Func<string> Description => () => "- You can plant bombs on players and force them to kill others\n- If the player is unable to kill someone within " +
        $"{CustomGameOptions.EnforceDur}s, the bomb will detonate and kill everyone within a {CustomGameOptions.EnforceRadius}m radius\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.DropsItems;

    public Enforcer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderKill;
        BombedPlayer = null;
        BombButton = new(this, "Enforce", AbilityTypes.Target, "Secondary", Bomb, CustomGameOptions.EnforceCd, CustomGameOptions.EnforceDur, BoomStart, UnBoom, CustomGameOptions.EnforceDelay,
            Exception1);
    }

    public void BoomStart()
    {
        if (CustomPlayer.Local == BombedPlayer && !IsDead)
        {
            Flash(Color);
            GetRole(BombedPlayer).Bombed = true;
        }
    }

    public void UnBoom()
    {
        if (!BombSuccessful)
            Explode();

        GetRole(BombedPlayer).Bombed = false;
        BombedPlayer = null;
        BombSuccessful = false;
    }

    private void Explode()
    {
        foreach (var player in GetClosestPlayers(BombedPlayer.transform.position, CustomGameOptions.EnforceRadius))
        {
            Spread(BombedPlayer, player);

            if (player.IsVesting() || player.IsProtected() || player.IsOnAlert() || player.IsShielded() || player.IsRetShielded() || player.IsProtectedMonarch() ||
                player.Is(LayerEnum.Pestilence))
            {
                continue;
            }

            RpcMurderPlayer(Player, player, DeathReasonEnum.Bombed, false);
        }
    }

    public void Bomb()
    {
        var interact = Interact(Player, BombButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            BombedPlayer = BombButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, BombButton, BombedPlayer);
            BombButton.Begin();
        }
        else if (interact.Reset)
            BombButton.StartCooldown(CooldownType.Reset);
        else if (interact.Protected)
            BombButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public bool Exception1(PlayerControl player) => player == BombedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction
        != SubFaction.None) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BombButton.Update2("SET BOMB");
    }

    public override void TryEndEffect() => BombButton.Update3((BombedPlayer != null && BombedPlayer.HasDied()) || IsDead || BombSuccessful);

    public override void ReadRPC(MessageReader reader) => BombedPlayer = reader.ReadPlayer();
}