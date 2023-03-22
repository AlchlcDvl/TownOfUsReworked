using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Engineer : CrewRole
    {
        public AbilityButton FixButton;
        public int UsesLeft = CustomGameOptions.MaxFixes;
        public bool ButtonUsable => UsesLeft > 0;
        public DateTime LastFixed;

        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            StartText = "Just Fix It";
            AbilitiesText = $"- You can fix sabotages at any time during the game.\n- You can vent.\n- You have {UsesLeft} fixes left.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
            RoleType = RoleEnum.Engineer;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.DifferentLens;
        }

        public float FixTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFixed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.FixCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}