using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using System.Collections.Generic;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Miner : IntruderRole
    {
        public AbilityButton MineButton;
        public DateTime LastMined;
        public bool CanPlace;

        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            StartText = "From The Top, Make It Drop, Boom, That's A Vent";
            AbilitiesText = $"- You can mine a vent, forming a vent system of your own\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Miner : Colors.Intruder;
            RoleType = RoleEnum.Miner;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
        }

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMined;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MineCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}