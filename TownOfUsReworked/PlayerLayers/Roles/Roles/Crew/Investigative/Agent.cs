using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using TMPro;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Agent : CrewRole
    {
        public Dictionary<byte, TMP_Text> PlayerNumbers = new();

        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            StartText = "Snoop Around And Find Stuff Out";
            AbilitiesText = "- You can see which colors are where on the admin table.\n- On Vitals, the time of death for each player will be shown.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Agent : Colors.Crew;
            Type = RoleEnum.Agent;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            PlayerNumbers = new();
        }
    }
}