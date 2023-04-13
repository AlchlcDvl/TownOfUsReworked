using TownOfUsReworked.Data;
using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Teleporter : IntruderRole
    {
        public AbilityButton TeleportButton;
        public DateTime LastTeleport;
        public DateTime LastMarked;
        public AbilityButton MarkButton;
        public bool CanMark;
        public Vector3 TeleportPoint = new(0, 0, 0);

        public Teleporter(PlayerControl player) : base(player)
        {
            Name = "Teleporter";
            StartText = "X Marks The Spot";
            AbilitiesText = $"- You can mark a spot to teleport to later\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Teleporter : Colors.Intruder;
            RoleType = RoleEnum.Teleporter;
            AlignmentName = IS;
        }

        public float TeleportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTeleport;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.TeleportCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMarked;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MarkCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}