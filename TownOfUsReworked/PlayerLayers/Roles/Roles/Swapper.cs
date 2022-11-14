using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<bool> ListOfActives = new List<bool>();
        public bool SwapWin;

        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            ImpostorText = () => "Swap The Votes Of Two People";
            TaskText = () => "Swap two people's votes to save the <color=#8BFDFDFF>Crew</color>!";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Swapper : Colors.Crew;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Swapper;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = () => "Crew (Sovereign)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.ShiftSwapSKDrac;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}