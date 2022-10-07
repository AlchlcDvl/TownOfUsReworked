using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class TimeMaster : Role
    {
        public KillButton _freezeButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastFrozen { get; set; }
        public System.Collections.Generic.Dictionary<byte, float> freezeList = new System.Collections.Generic.Dictionary<byte, float>();

        public TimeMaster(PlayerControl player) : base(player)
        {
            Name = "Time Master";
            ImpostorText = () => "Freeze Time To Stop The <color=#8BFDFDFF>Crew</color>";
            TaskText = () => "Freeze time to stop the <color=#8BFDFDFF>Crew</color> from moving";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.TimeMaster;
            else Color = Patches.Colors.Impostor;
            LastFrozen = DateTime.UtcNow;
            RoleType = RoleEnum.TimeMaster;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            AddToRoleHistory(RoleType);
        }

        public bool Frozen => TimeRemaining > 0f;
        
        public KillButton FreezeButton
        {
            get => _freezeButton;
            set
            {
                _freezeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFrozen;
            var num = CustomGameOptions.FreezeDuration * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TimeFreeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.TimeFreeze();
        }

        public void TimeUnfreeze()
        {
            Enabled = false;
            LastFrozen = DateTime.UtcNow;
            Utils.TimeUnfreeze();
        }
    }
}