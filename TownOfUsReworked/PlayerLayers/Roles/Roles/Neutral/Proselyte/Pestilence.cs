using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Pestilence : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public bool PestilenceWins;
        public AbilityButton ObliterateButton;

        public Pestilence(PlayerControl owner) : base(owner)
        {
            Name = "Pestilence";
            StartText = "The Horseman Of The Apocalypse Has Arrived!";
            AbilitiesText = "Kill everyone with your unstoppable abilities!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Pestilence : Colors.Neutral;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Pestilence;
            RoleAlignment = RoleAlignment.NeutralPros;
            AlignmentName = NP;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.PestKillCd) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}