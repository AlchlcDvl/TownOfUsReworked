using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ambusher : IntruderRole
    {
        public bool Enabled;
        public DateTime LastAmbushed;
        public float TimeRemaining;
        public bool OnAmbush => TimeRemaining > 0f;
        public PlayerControl AmbushedPlayer;
        public PlayerControl ClosestAmbush;
        public AbilityButton AmbushButton;

        public Ambusher(PlayerControl player) : base(player)
        {
            Name = "Ambusher";
            StartText = "Ambush";
            AbilitiesText = $"- You can ambush players.\n- Ambushed players will be forced to be on alert and will kill whoever interacts with then.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Ambusher : Colors.Intruder;
            RoleType = RoleEnum.Ambusher;
            RoleAlignment = RoleAlignment.IntruderKill;
            AlignmentName = IK;
            RoleDescription = "You are a Ambusher! You are a patient killer who can strike at any given moment! Kill as many people as possible using your skill and dominate the game!";
            InspectorResults = InspectorResults.TracksOthers;
        }

        public float AmbushTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAmbushed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.AmbushCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ambush()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || AmbushedPlayer.Data.IsDead || AmbushedPlayer.Data.Disconnected || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnAmbush()
        {
            Enabled = false;
            LastAmbushed = DateTime.UtcNow;
            AmbushedPlayer = null;
        }
    }
}