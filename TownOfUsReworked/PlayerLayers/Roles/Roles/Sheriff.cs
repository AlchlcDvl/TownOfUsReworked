using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Sheriff : Role
    {
        public List<byte> Interrogated = new List<byte>();
        public bool SherWin;
        public bool UsedThisRound { get; set; } = false;
        public int randomSheriffAccuracy = 100;
        public PlayerControl ClosestPlayer;
        public DateTime LastInterrogated { get; set; }

        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            ImpostorText = () => "Reveal The Alignment Of Other Players";
            TaskText = () => "Reveal alignments of other players to find the <color=#FF0000FF>Intruders</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Sheriff : Colors.Crew;
            SubFaction = SubFaction.None;
            LastInterrogated = DateTime.UtcNow;
            RoleType = RoleEnum.Sheriff;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = () => "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.SherConsigInspBm;
            AddToRoleHistory(RoleType);
        }

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInterrogated;
            var num = CustomGameOptions.InterrogateCd * 1000f;
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