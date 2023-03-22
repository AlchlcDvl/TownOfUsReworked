using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Betrayer : NeutralRole
    {
        public AbilityButton KillButton;
        public DateTime LastKilled;
        public PlayerControl ClosestPlayer = null;

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
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BetrayerKillCooldown) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}