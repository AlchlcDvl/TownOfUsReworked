using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Sheriff : CrewRole
    {
        public List<byte> Interrogated = new();
        public PlayerControl ClosestPlayer;
        public AbilityButton InterrogateButton;
        public DateTime LastInterrogated;

        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            StartText = "Reveal The Alignment Of Other Players";
            AbilitiesText = "- You can reveal alignments of other players relative to the <color=#8BFDFDFF>Crew</color>.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Sheriff : Colors.Crew;
            RoleType = RoleEnum.Sheriff;
            Interrogated = new();
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = CI;
            InspectorResults = InspectorResults.HasInformation;
        }

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInterrogated;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.InterrogateCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}