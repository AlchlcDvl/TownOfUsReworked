using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Miner : IntruderRole
    {
        public readonly List<Vent> Vents = new List<Vent>();
        public AbilityButton MineButton;
        public DateTime LastMined;
        public bool CanPlace;

        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            StartText = "From The Top, Make It Drop, Boom, That's A Vent";
            AbilitiesText = "Place vents around the map";
            Color = CustomGameOptions.CustomIntColors ? Colors.Miner : Colors.Intruder;
            LastMined = DateTime.UtcNow;
            RoleType = RoleEnum.Miner;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
        }

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMined;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MineCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}