using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Detective : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined;
        public AbilityButton ExamineButton;

        public Detective(PlayerControl player) : base(player)
        {
            Name = "Detective";
            StartText = "Examine Players To Find Bloody Hands";
            AbilitiesText = "- You can examine players to see if they have killed recently.\n- Your screen will flash red if your target has killed in the " +
                $"last {CustomGameOptions.RecentKill}s.\n- You can view everyone's footprints to see where they go or where they came from.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Detective : Colors.Crew;
            RoleType = RoleEnum.Detective;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            InspectorResults = InspectorResults.HasInformation;
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastExamined;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ExamineCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}