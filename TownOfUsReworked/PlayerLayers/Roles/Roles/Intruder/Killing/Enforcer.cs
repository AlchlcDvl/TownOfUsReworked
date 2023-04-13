using TownOfUsReworked.Data;
using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Enforcer : IntruderRole
    {
        public AbilityButton BombButton;
        public PlayerControl BombedPlayer;
        public PlayerControl ClosestBomb;
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
                $", the bomb will detonate and kill everyone within a {CustomGameOptions.EnforceRadius}m\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Enforcer : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderKill;
            AlignmentName = IK;
        }

        public float BombTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBombed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.EnforceCooldown, CustomGameOptions.MafiosoAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Boom()
        {
            if (!Enabled && PlayerControl.LocalPlayer == BombedPlayer)
            {
                Utils.Flash(Color, "There's a bomb on you!", 2);
                GetRole(BombedPlayer).Bombed = true;
            }

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || MeetingHud.Instance || BombSuccessful)
                TimeRemaining = 0f;
        }

        public void Delay()
        {
            TimeRemaining2 -= Time.deltaTime;

            if (Player.Data.IsDead || MeetingHud.Instance)
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
                    Utils.RpcMurderPlayer(BombedPlayer, player, DeathReasonEnum.Bombed, false);
            }
        }
    }
}