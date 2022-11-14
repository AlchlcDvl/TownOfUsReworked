using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Shifter : Role
    {
        public bool ShiftWin;
        public PlayerControl ClosestPlayer;
        public DateTime LastShifted { get; set; }

        public Shifter(PlayerControl player) : base(player)
        {
            Name = "Shifter";
            ImpostorText = () => "Shift around different roles";
            TaskText = () => "Steal other people's roles";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Shifter : Colors.Crew;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Shifter;
            LastShifted = DateTime.UtcNow;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = () => "Crew (Support)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.ShiftSwapSKDrac;
            AddToRoleHistory(RoleType);
        }

        public float ShifterShiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastShifted;
            var num = CustomGameOptions.ShifterCd * 1000f;
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