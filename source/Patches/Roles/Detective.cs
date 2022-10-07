using System;

namespace TownOfUs.Roles
{
    public class Detective : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined { get; set; }

        public Detective(PlayerControl player) : base(player)
        {
            Name = "Detective";
            ImpostorText = () => "Examine Players To Find Bloody Hands";
            TaskText = () => "Examine suspicious players to find evildoers";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Detective;
            else Color = Patches.Colors.Crew;
            LastExamined = DateTime.UtcNow;
            RoleType = RoleEnum.Detective;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            AddToRoleHistory(RoleType);
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.ExamineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}