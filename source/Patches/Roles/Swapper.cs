using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<bool> ListOfActives = new List<bool>();


        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            ImpostorText = () => "Swap The Votes Of Two People";
            TaskText = () => "Swap two people's votes to save the <color=#8BFDFDFF>Crew</color>!";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Swapper;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Swapper;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewSov;
            AlignmentName = "Crew (Sovereign)";
            AddToRoleHistory(RoleType);
        }
    }
}