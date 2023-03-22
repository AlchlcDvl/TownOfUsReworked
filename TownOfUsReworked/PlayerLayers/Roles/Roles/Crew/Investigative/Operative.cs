using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Objects;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Operative : CrewRole
    {
        public List<Bug> Bugs;
        public DateTime LastBugged;
        public int UsesLeft;
        public List<RoleEnum> BuggedPlayers;
        public bool ButtonUsable => UsesLeft > 0;
        public AbilityButton BugButton;

        public Operative(PlayerControl player) : base(player)
        {
            Name = "Operative";
            StartText = "Detect Which Roles Are Here";
            AbilitiesText = "- You can place bugs around the map.\n- Upon triggering the bugs, the player's role will be included in a list to bw shown in the next meeting." + 
                $"\n- You have {UsesLeft} bugs left.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Operative : Colors.Crew;
            RoleType = RoleEnum.Operative;
            BuggedPlayers = new List<RoleEnum>();
            UsesLeft = CustomGameOptions.MaxBugs;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            Bugs = new List<Bug>();
            InspectorResults = InspectorResults.DropsItems;
        }

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBugged;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
