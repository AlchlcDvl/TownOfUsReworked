using System;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

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
            AbilitiesText = $"- You can put on a vest, which makes you unkillable for a short duration of time.\n- You have {UsesLeft} vests remaining.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Survivor : Colors.Neutral;
            LastVested = DateTime.UtcNow;
            RoleType = RoleEnum.Survivor;
            UsesLeft = CustomGameOptions.MaxVests;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            Objectives = "- Live to the end of the game.";
            InspectorResults = InspectorResults.SeeksToProtect;
            RoleDescription = "You are a Survivor! You care not for who lives or dies, you just want to live! Do whatever it takes to survive, even it's with the bad guys.";
        }

        public float VestTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastVested;
            var num = CustomGameOptions.VestCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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