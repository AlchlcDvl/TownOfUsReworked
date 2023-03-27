using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Shifter : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastShifted;
        public AbilityButton ShiftButton;

        public Shifter(PlayerControl player) : base(player)
        {
            Name = "Shifter";
            StartText = "Shift Around Roles";
            AbilitiesText = "- You can steal another player's role.\n- You can only shift with <color=#8BFDFDFF>Crew</color>.\n- Shifting with non-<color=#8BFDFDFF>Crew</color> will " +
                "cause you to kill yourself.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Shifter : Colors.Crew;
            RoleType = RoleEnum.Shifter;
            LastShifted = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.BringsChaos;
        }

        public float ShiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastShifted;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ShifterCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}