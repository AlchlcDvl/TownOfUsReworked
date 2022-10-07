using System;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Sheriff : Role
    {
        public List<byte> Interrogated = new List<byte>();

        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            ImpostorText = () => "Reveal The Alignment Of Other Players";
            TaskText = () => "Reveal alignments of other players to find the <color=#FF0000FF>Intruders</color>";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Sheriff;
            else Color = Patches.Colors.Crew;
            LastInterrogated = DateTime.UtcNow;
            RoleType = RoleEnum.Sheriff;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewKill;
            AlignmentName = "Crew (Killing)";
            AddToRoleHistory(RoleType);
        }

        public bool UsedThisRound { get; set; } = false;
        public int randomSheriffAccuracy = 100;
        public PlayerControl ClosestPlayer;
        public DateTime LastInterrogated { get; set; }

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInterrogated;
            var num = CustomGameOptions.InterrogateCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}