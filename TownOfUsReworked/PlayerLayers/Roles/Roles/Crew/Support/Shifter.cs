using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

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
            AbilitiesText = "- You can steal another player's role.\n- You can only shift with <color=#8BFDFDFF>Crew</color>.\n- Shifting with non-<color=#8BFDFDFF>Crew</color> will cause" +
                " you to kill yourself.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Shifter : Colors.Crew;
            RoleType = RoleEnum.Shifter;
            LastShifted = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            RoleDescription = "Your are a Shifter! You are a rogue alien who can swap roles! Steal a different player's role to be useful to the Crew!";
            InspectorResults = InspectorResults.BringsChaos;
            //IntroSound = "ShifterIntro";
        }

        public float ShiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastShifted;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ShifterCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}