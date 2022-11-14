using System;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Veteran : Role
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
        public bool VetWin;
        public bool OnAlert => TimeRemaining > 0f;

        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            ImpostorText = () => "Alert To Kill Anyone Who Touches You";
            TaskText = () => "Alert to kill whoever interacts with you";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Veteran : Colors.Crew;
            SubFaction = SubFaction.None;
            LastAlerted = DateTime.UtcNow;
            RoleType = RoleEnum.Veteran;
            Faction = Faction.Crew;
            UsesLeft = CustomGameOptions.MaxAlerts;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = () => "Crew (Killing)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.WraithDetGrenVet;
            AddToRoleHistory(RoleType);
        }

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAlerted;
            var num = CustomGameOptions.AlertCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}