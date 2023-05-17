namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Enforcer : IntruderRole
    {
        public CustomButton BombButton;
        public PlayerControl BombedPlayer;
        public bool Enabled;
        public DateTime LastBombed;
        public float TimeRemaining;
        public float TimeRemaining2;
        public bool Bombing => TimeRemaining > 0f;
        public bool DelayActive => TimeRemaining2 > 0f;
        public bool BombSuccessful;

        public Enforcer(PlayerControl player) : base(player)
        {
            Name = "Enforcer";
            RoleType = RoleEnum.Enforcer;
            StartText = "Plant Bombs On Players And Force Them To Kill";
            AbilitiesText = $"- You can plant bombs on players and force them to kill others\n- If the player is unable to kill someone within {CustomGameOptions.EnforceDuration}s" +
                $", the bomb will detonate and kill everyone within a {CustomGameOptions.EnforceRadius}m radius\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Enforcer : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderKill;
            Type = LayerEnum.Enforcer;
            BombedPlayer = null;
            BombButton = new(this, "Enforce", AbilityTypes.Direct, "Secondary", Bomb, Exception1);
            InspectorResults = InspectorResults.DropsItems;
        }

        public float BombTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBombed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.EnforceCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Boom()
        {
            if (!Enabled && PlayerControl.LocalPlayer == BombedPlayer)
            {
                Utils.Flash(Color);
                GetRole(BombedPlayer).Bombed = true;
            }

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (IsDead || MeetingHud.Instance || BombedPlayer.Data.IsDead || BombedPlayer.Data.Disconnected || BombSuccessful)
                TimeRemaining = 0f;
        }

        public void Delay()
        {
            TimeRemaining2 -= Time.deltaTime;

            if (IsDead || MeetingHud.Instance || BombedPlayer.Data.IsDead || BombedPlayer.Data.Disconnected)
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
            foreach (var player in Utils.GetClosestPlayers(BombedPlayer.GetTruePosition(), CustomGameOptions.EnforceRadius))
            {
                Utils.Spread(BombedPlayer, player);

                if (player.IsVesting() || player.IsProtected() || player.IsOnAlert() || player.IsShielded() || player.IsRetShielded())
                    continue;

                if (!player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(Player, player, DeathReasonEnum.Bombed, false);
            }
        }

        public void Bomb()
        {
            if (BombTimer() != 0f || Utils.IsTooFar(Player, BombButton.TargetPlayer) || BombedPlayer == BombButton.TargetPlayer)
                return;

            var interact = Utils.Interact(Player, BombButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetBomb);
                writer.Write(PlayerId);
                writer.Write(BombButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.EnforceDuration;
                TimeRemaining2 = CustomGameOptions.EnforceDelay;
                BombedPlayer = BombButton.TargetPlayer;
                Delay();
            }
            else if (interact[0])
                LastBombed = DateTime.UtcNow;
            else if (interact[1])
                LastBombed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == BombedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BombButton.Update("BOMB", BombTimer(), CustomGameOptions.EnforceCooldown, DelayActive || Bombing, DelayActive ? TimeRemaining2 : TimeRemaining, DelayActive ?
                CustomGameOptions.EnforceDelay : CustomGameOptions.EnforceDuration);
        }
    }
}