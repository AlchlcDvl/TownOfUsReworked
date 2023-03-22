using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using System;
using TownOfUsReworked.Classes;
using Hazel;

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
            TaskText = "- Kill everyone!";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Corrupted : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Corrupted;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.CorruptedKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}