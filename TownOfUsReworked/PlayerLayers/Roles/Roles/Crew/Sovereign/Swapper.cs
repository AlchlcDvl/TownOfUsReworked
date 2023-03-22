using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Swapper : CrewRole
    {
        public readonly List<GameObject> MoarButtons;
        public readonly List<bool> ListOfActives;

        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            StartText = "Swap Votes For Maximum Chaos";
            AbilitiesText = "- You can swap the votes against 2 players in meetings.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Swapper : Colors.Crew;
            RoleType = RoleEnum.Swapper;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            InspectorResults = InspectorResults.BringsChaos;
            MoarButtons = new List<GameObject>();
            ListOfActives = new List<bool>();
        }
    }
}