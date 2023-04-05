using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class TimeMaster : IntruderRole
    {
        public AbilityButton FreezeButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastFrozen;
        public bool Frozen => TimeRemaining > 0f;

        public TimeMaster(PlayerControl player) : base(player)
        {
            Name = "Time Master";
            StartText = "Freeze Time To Stop The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = $"- You can freeze time to stop the <color=#8BFDFDFF>Crew</color> from moving\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.TimeMaster : Colors.Intruder;
            RoleType = RoleEnum.TimeMaster;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
        }

        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFrozen;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.FreezeCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TimeFreeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void Unfreeze()
        {
            Enabled = false;
            LastFrozen = DateTime.UtcNow;
            Freeze.UnfreezeAll();
        }
    }
}