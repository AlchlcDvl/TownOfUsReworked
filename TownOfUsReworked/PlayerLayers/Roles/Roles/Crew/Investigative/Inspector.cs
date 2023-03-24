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
        public List<byte> Inspected = new();
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
            Inspected = new();
            InspectorResults = InspectorResults.HasInformation;
        }

        public float InspectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInspected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}