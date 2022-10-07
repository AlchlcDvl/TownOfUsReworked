using System;

namespace TownOfUs.Roles
{
    public class Vigilante : Role
    {
        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            ImpostorText = () => "Shoot The <color=#FF0000FF>Intruders</color>";
            TaskText = () => "Kill the <color=#FF0000FF>Intruders</color> but not the <color=#8BFDFDFF>Crew</color>";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Vigilante;
            else Color = Patches.Colors.Crew;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Vigilante;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewKill;
            AlignmentName = "Crew (Killing)";
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }
        public bool FirstRound { get; set; } = false;

        public float VigilanteKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.VigiKillCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}