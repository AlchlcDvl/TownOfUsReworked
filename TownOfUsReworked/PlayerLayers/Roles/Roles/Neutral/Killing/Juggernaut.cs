using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Juggernaut : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public int JuggKills;
        public AbilityButton AssaultButton;

        public Juggernaut(PlayerControl player) : base(player)
        {
            Name = "Juggernaut";
            StartText = "Your Power Grows With Every Kill";
            AbilitiesText = "With each kill your kill cooldown decreases\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Juggernaut : Colors.Neutral;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Juggernaut;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            JuggKills = 0;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Mathf.Clamp(Utils.GetModifiedCooldown(CustomGameOptions.JuggKillCooldown, -(CustomGameOptions.JuggKillBonus * JuggKills)), 5, CustomGameOptions.JuggKillCooldown) *
                1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}