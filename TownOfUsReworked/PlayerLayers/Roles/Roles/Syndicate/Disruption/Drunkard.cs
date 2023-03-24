using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Drunkard : SyndicateRole
    {
        public AbilityButton ConfuseButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastConfused;
        public bool Confused => TimeRemaining > 0f;

        public Drunkard(PlayerControl player) : base(player)
        {
            Name = "Drunkard";
            StartText = "Confuse The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = "Confuse the <color=#8BFDFDFF>Crew</color>";
            Color = CustomGameOptions.CustomSynColors ? Colors.Drunkard : Colors.Syndicate;
            LastConfused = DateTime.UtcNow;
            RoleType = RoleEnum.Drunkard;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
        }

        public float DrunkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastConfused;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.FreezeCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Confuse()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unconfuse()
        {
            Enabled = false;
            LastConfused = DateTime.UtcNow;
            Reverse.ConfuseFunctions.UnconfuseAll();
        }
    }
}