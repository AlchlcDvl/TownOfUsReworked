using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Corrupted : Objectifier
    {
        public DateTime LastKilled;
        public PlayerControl ClosestPlayer;
        public AbilityButton KillButton;

        public Corrupted(PlayerControl player) : base(player)
        {
            Name = "Corrupted";
            SymbolName = "δ";
            TaskText = "- Kill everyone";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Corrupted : Colors.Objectifier;
            Type = ObjectifierEnum.Corrupted;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomGameOptions.CorruptedKillCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}