using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;
using System;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Betrayer : NeutralRole
    {
        public AbilityButton KillButton;
        public DateTime LastKilled;
        public PlayerControl ClosestPlayer;

        public Betrayer(PlayerControl player) : base(player)
        {
            Name = "Betrayer";
            RoleType = RoleEnum.Betrayer;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Betrayer : Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralPros;
            AlignmentName = NP;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.BetrayerKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}