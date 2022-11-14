using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Vigilante : Role
    {
        public bool VigWin;
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            ImpostorText = () => "Shoot The <color=#FF0000FF>Intruders</color>";
            TaskText = () => "Kill the <color=#FF0000FF>Intruders</color> but not the <color=#8BFDFDFF>Crew</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Vigilante : Colors.Crew;
            SubFaction = SubFaction.None;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Vigilante;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = () => "Crew (Killing)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.SurvVHVampVig;
            AddToRoleHistory(RoleType);
        }

        public float VigilanteKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.VigiKillCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}