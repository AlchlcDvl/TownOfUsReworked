using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Thief : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastStolen;
        public AbilityButton StealButton;

        public Thief(PlayerControl player) : base(player)
        {
            Name = "Thief";
            StartText = "Steal From The Killers";
            AbilitiesText = "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Thief : Colors.Neutral;
            LastStolen = DateTime.UtcNow;
            RoleType = RoleEnum.Thief;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStolen;
            var num = CustomGameOptions.ThiefKillCooldown * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}