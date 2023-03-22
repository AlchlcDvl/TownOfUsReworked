using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class SerialKiller : NeutralRole
    {
        public AbilityButton BloodlustButton;
        public AbilityButton StabButton;
        public bool Enabled;
        public PlayerControl ClosestPlayer;
        public DateTime LastLusted;
        public DateTime LastKilled;
        public float TimeRemaining;
        public bool Lusted => TimeRemaining > 0f;

        public SerialKiller(PlayerControl player) : base(player)
        {
            Name = "Serial Killer";
            StartText = "You Like To Play With Knives";
            AbilitiesText = "Engage in your bloodlust to kill everyone";
            Color = CustomGameOptions.CustomNeutColors ? Colors.SerialKiller : Colors.Neutral;
            RoleType = RoleEnum.SerialKiller;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
        }

        public float LustTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastLusted;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BloodlustCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Bloodlust()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unbloodlust()
        {
            Enabled = false;
            LastLusted = DateTime.UtcNow;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.LustKillCd) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
