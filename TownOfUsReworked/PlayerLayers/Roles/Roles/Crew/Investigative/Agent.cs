using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using TMPro;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Agent : CrewRole
    {
        public Dictionary<byte, TMP_Text> PlayerNumbers;

        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            StartText = "Snoop Around And Find Stuff Out";
            AbilitiesText = "- You can see which colors are where on the admin table.\n- On Vitals, the time of death for each player will be shown.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Agent : Colors.Crew;
            RoleType = RoleEnum.Agent;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            PlayerNumbers = new Dictionary<byte, TMP_Text>();
            //IntroSound = "AgentIntro";
            RoleDescription = "Your are an Agent! You can see extra information from the admin table and the vitals screen. When active, all " +
                "players in detectable rooms will have their color revealed to you. The Vitals screen will show you how long has someone been dead for.";
        }
    }
}