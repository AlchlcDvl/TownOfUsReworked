using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Pestilence : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public AbilityButton ObliterateButton;

        public Pestilence(PlayerControl owner) : base(owner)
        {
            Name = "Pestilence";
            StartText = "The Horseman Of The Apocalypse Has Arrived!";
            AbilitiesText = "Kill everyone with your unstoppable abilities!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Pestilence : Colors.Neutral;
            LastKilled = DateTime.UtcNow;
            Type = RoleEnum.Pestilence;
            RoleAlignment = RoleAlignment.NeutralPros;
            AlignmentName = NP;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.PestKillCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}