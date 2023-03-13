using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Sheriff : CrewRole
    {
        public List<byte> Interrogated;
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
            Interrogated = new List<byte>();
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = CI;
            RoleDescription = "You are a Sheriff! You are a law enforcement officer who can investigate players to see if they are capable of harming the Crew or not!";
            InspectorResults = InspectorResults.HasInformation;
        }

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInterrogated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InterrogateCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}