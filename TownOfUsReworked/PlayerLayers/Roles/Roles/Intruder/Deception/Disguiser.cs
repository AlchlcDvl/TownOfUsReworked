using System;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Disguiser : IntruderRole
    {
        public AbilityButton DisguiseButton;
        public AbilityButton MeasureButton;
        public DateTime LastDisguised;
        public DateTime LastMeasured;
        public PlayerControl MeasuredPlayer;
        public float TimeRemaining;
        public float TimeRemaining2;
        public PlayerControl DisguisedPlayer;
        public PlayerControl ClosestTarget;
        public bool DelayActive => TimeRemaining2 > 0f;
        public bool Disguised => TimeRemaining > 0f;
        public bool Enabled;

        public Disguiser(PlayerControl player) : base(player)
        {
            Name = "Disguiser";
            StartText = "Disguise The <color=#8BFDFDFF>Crew</color> To Frame Them";
            AbilitiesText = "- You can disguise a player into someone else's appearance.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Disguiser : Colors.Intruder;
            RoleType = RoleEnum.Disguiser;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
        }

        public void Disguise()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(DisguisedPlayer, MeasuredPlayer);
            Enabled = true;

            if (Player.Data.IsDead || DisguisedPlayer.Data.IsDead || DisguisedPlayer.Data.Disconnected || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Delay()
        {
            TimeRemaining2 -= Time.deltaTime;

            if (Player.Data.IsDead || DisguisedPlayer.Data.IsDead || DisguisedPlayer.Data.Disconnected)
                TimeRemaining2 = 0f;
        }

        public void UnDisguise()
        {
            Enabled = false;
            Utils.DefaultOutfit(DisguisedPlayer);
            LastDisguised = DateTime.UtcNow;

            if (CustomGameOptions.DisgCooldownsLinked)
                LastMeasured = DateTime.UtcNow;
        }

        public float DisguiseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDisguised;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.DisguiseCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MeasureTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMeasured;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MeasureCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}
