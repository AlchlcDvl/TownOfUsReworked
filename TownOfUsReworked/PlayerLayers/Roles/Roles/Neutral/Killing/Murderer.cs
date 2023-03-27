using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;

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
            AbilitiesText = "Kill everyone!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Murderer : Colors.Neutral;
            LastKilled = DateTime.UtcNow;
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