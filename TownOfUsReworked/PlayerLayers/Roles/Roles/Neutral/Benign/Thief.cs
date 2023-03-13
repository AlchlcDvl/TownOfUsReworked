using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

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
            AbilitiesText = "- You can kill players to steal their roles.\n- You cannot steal roles from players who cannot kill.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Thief : Colors.Neutral;
            LastStolen = DateTime.UtcNow;
            RoleType = RoleEnum.Thief;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStolen;
            var num = CustomGameOptions.ThiefKillCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}