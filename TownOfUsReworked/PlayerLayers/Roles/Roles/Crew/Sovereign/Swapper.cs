using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Swapper : CrewRole
    {
        public readonly List<GameObject> MoarButtons = new();
        public readonly List<bool> ListOfActives = new();

        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            StartText = "Swap Votes For Maximum Chaos";
            AbilitiesText = "- You can swap the votes against 2 players in meetings";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Swapper : Colors.Crew;
            RoleType = RoleEnum.Swapper;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            InspectorResults = InspectorResults.BringsChaos;
            MoarButtons = new();
            ListOfActives = new();
            Type = LayerEnum.Swapper;
        }
    }
}