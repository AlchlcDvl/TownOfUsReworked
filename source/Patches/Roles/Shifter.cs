using System;

namespace TownOfUs.Roles
{
    public class Shifter : Role
    {
        public Shifter(PlayerControl player) : base(player)
        {
            Name = "Shifter";
            ImpostorText = () => "Shift around different roles";
            TaskText = () => "Steal other people's roles";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Shifter;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Shifter;
            LastShifted = DateTime.UtcNow;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewSupport;
            AlignmentName = "Crew (Support)";
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastShifted { get; set; }

        public float ShifterShiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastShifted;
            var num = CustomGameOptions.ShifterCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}