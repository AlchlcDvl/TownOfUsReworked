using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Murderer : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public AbilityButton MurderButton;

        public Murderer(PlayerControl player) : base(player)
        {
            Name = "Murderer";
            StartText = "Imagine Getting Boring Murderer";
            Objectives = "- Murder anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Murderer : Colors.Neutral;
            RoleType = RoleEnum.Murderer;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MurdKCD) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}