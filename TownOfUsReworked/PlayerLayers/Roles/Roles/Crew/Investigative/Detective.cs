using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

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
            var timeSpan = utcNow - LastExamined;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ExamineCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}