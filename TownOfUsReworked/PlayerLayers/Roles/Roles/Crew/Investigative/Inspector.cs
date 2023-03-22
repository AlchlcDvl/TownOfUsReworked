using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Il2CppSystem.Collections.Generic;
using System;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Inspector : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastInspected;
        public List<byte> Inspected;
        public AbilityButton InspectButton;

        public Inspector(PlayerControl player) : base(player)
        {
            Name = "Inspector";
            RoleType = RoleEnum.Inspector;
            StartText = "Inspect Player For Their Roles";
            AbilitiesText = "- You can check a player to get a role list of what they could be.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            Inspected = new List<byte>();
            InspectorResults = InspectorResults.HasInformation;
        }

        public float InspectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInspected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}