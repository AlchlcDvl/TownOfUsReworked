using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Survivor : NeutralRole
    {
        public bool Enabled;
        public DateTime LastVested;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Vesting => TimeRemaining > 0f;
        public bool Alive => !Player.Data.Disconnected && !Player.Data.IsDead;
        public AbilityButton VestButton;

        public Survivor(PlayerControl player) : base(player)
        {
            Name = "Survivor";
            StartText = "Do Whatever It Takes To Live";
            AbilitiesText = "- You can put on a vest, which makes you unkillable for a short duration of time";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Survivor : Colors.Neutral;
            RoleType = RoleEnum.Survivor;
            UsesLeft = CustomGameOptions.MaxVests;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            Objectives = "- Live to the end of the game";
            InspectorResults = InspectorResults.SeeksToProtect;
        }

        public float VestTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastVested;
            var num = CustomGameOptions.VestCd * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Vest()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnVest()
        {
            Enabled = false;
            LastVested = DateTime.UtcNow;
        }
    }
}