namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Enforcer : Intruder
    {
        public CustomButton BombButton { get; set; }
        public PlayerControl BombedPlayer { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastBombed { get; set; }
        public float TimeRemaining { get; set; }
        public float TimeRemaining2 { get; set; }
        public bool Bombing => TimeRemaining > 0f;
        public bool DelayActive => TimeRemaining2 > 0f;
        public bool BombSuccessful { get; set; }

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Enforcer : Colors.Intruder;
        public override string Name => "Enforcer";
        public override LayerEnum Type => LayerEnum.Enforcer;
        public override RoleEnum RoleType => RoleEnum.Enforcer;
        public override Func<string> StartText => () => "Force The <color=#8CFFFFFF>Crew</color> To Do Your Bidding";
        public override Func<string> Description => () => "- You can plant bombs on players and force them to kill others\n- If the player is unable to kill someone within " +
            $"{CustomGameOptions.EnforceDuration}s, the bomb will detonate and kill everyone within a {CustomGameOptions.EnforceRadius}m radius\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.DropsItems;
        public float Timer => ButtonUtils.Timer(Player, LastBombed, CustomGameOptions.EnforceCooldown);

        public Enforcer(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderKill;
            BombedPlayer = null;
            BombButton = new(this, "Enforce", AbilityTypes.Direct, "Secondary", Bomb, Exception1);
        }

        public void Boom()
        {
            if (!Enabled && CustomPlayer.Local == BombedPlayer)
            {
                Flash(Color);
                GetRole(BombedPlayer).Bombed = true;
            }

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (IsDead || Meeting || BombedPlayer.Data.IsDead || BombedPlayer.Data.Disconnected || BombSuccessful)
                TimeRemaining = 0f;
        }

        public void Delay()
        {
            TimeRemaining2 -= Time.deltaTime;

            if (IsDead || Meeting || BombedPlayer.Data.IsDead || BombedPlayer.Data.Disconnected)
                TimeRemaining2 = 0f;
        }

        public void Unboom()
        {
            Enabled = false;
            LastBombed = DateTime.UtcNow;
            GetRole(BombedPlayer).Bombed = false;

            if (!BombSuccessful)
                Explode();

            BombedPlayer = null;
        }

        private void Explode()
        {
            foreach (var player in GetClosestPlayers(BombedPlayer.GetTruePosition(), CustomGameOptions.EnforceRadius))
            {
                Spread(BombedPlayer, player);

                if (player.IsVesting() || player.IsProtected() || player.IsOnAlert() || player.IsShielded() || player.IsRetShielded() || player.IsProtectedMonarch() ||
                    player.Is(RoleEnum.Pestilence))
                {
                    continue;
                }

                RpcMurderPlayer(Player, player, DeathReasonEnum.Bombed, false);
            }
        }

        public void Bomb()
        {
            if (Timer != 0f || IsTooFar(Player, BombButton.TargetPlayer) || BombedPlayer == BombButton.TargetPlayer)
                return;

            var interact = Interact(Player, BombButton.TargetPlayer);

            if (interact[3])
            {
                TimeRemaining = CustomGameOptions.EnforceDuration;
                TimeRemaining2 = CustomGameOptions.EnforceDelay;
                BombedPlayer = BombButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.SetBomb, this, BombedPlayer);
                Delay();
            }
            else if (interact[0])
                LastBombed = DateTime.UtcNow;
            else if (interact[1])
                LastBombed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == BombedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
            Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BombButton.Update("BOMB", Timer, CustomGameOptions.EnforceCooldown, DelayActive || Bombing, DelayActive ? TimeRemaining2 : TimeRemaining, DelayActive ?
                CustomGameOptions.EnforceDelay : CustomGameOptions.EnforceDuration);
        }
    }
}