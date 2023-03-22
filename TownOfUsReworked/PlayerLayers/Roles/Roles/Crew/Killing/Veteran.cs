using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Veteran : CrewRole
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool OnAlert => TimeRemaining > 0f;
        public AbilityButton AlertButton;

        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            StartText = "Alert To Kill Anyone Who Touches You";
            AbilitiesText = $"- You can go on alert.\n- When on alert, you will kill whoever interacts with you.\n- You can {UsesLeft} alerts left.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Veteran : Colors.Crew;
            RoleType = RoleEnum.Veteran;
            UsesLeft = CustomGameOptions.MaxAlerts;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = CK;
            InspectorResults = InspectorResults.SeeksToProtect;
        }

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAlerted;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }
    }
}