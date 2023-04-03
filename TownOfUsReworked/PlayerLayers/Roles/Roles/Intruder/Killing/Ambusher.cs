using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

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
            AbilitiesText = "- You can ambush players.\n- Ambushed players will be forced to be on alert and will kill whoever interacts with then.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Ambusher : Colors.Intruder;
            Type = RoleEnum.Ambusher;
            RoleAlignment = RoleAlignment.IntruderKill;
            AlignmentName = IK;
            InspectorResults = InspectorResults.TracksOthers;
        }

        public float AmbushTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAmbushed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.AmbushCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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