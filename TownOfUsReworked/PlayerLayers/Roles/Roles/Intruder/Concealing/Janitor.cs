using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Janitor : IntruderRole
    {
        public AbilityButton CleanButton;
        public DeadBody CurrentTarget;
        public DateTime LastCleaned;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            StartText = "Sanitise The Ship, By Any Means Neccessary";
            AbilitiesText = "- You can clean up dead bodies, making them disappear from sight.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Janitor : Colors.Intruder;
            RoleType = RoleEnum.Janitor;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = IC;
            InspectorResults = InspectorResults.MeddlesWithDead;
        }

        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCleaned;
            var num = Utils.GetModifiedCooldown(Utils.LastImp() && CustomGameOptions.SoloBoost ? (CustomGameOptions.JanitorCleanCd - CustomGameOptions.UnderdogKillBonus) :
                CustomGameOptions.JanitorCleanCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}