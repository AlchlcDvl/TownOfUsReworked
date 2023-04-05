using System;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Wraith : IntruderRole
    {
        public AbilityButton InvisButton;
        public bool Enabled;
        public DateTime LastInvis;
        public float TimeRemaining;
        public bool IsInvis => TimeRemaining > 0f;

        public Wraith(PlayerControl player) : base(player)
        {
            Name = "Wraith";
            StartText = "Sneaky Sneaky";
            AbilitiesText = $"- You can turn invisible\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Wraith : Colors.Intruder;
            LastInvis = DateTime.UtcNow;
            RoleType = RoleEnum.Wraith;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
        }

        public float InvisTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInvis;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.InvisCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Invis(Player, PlayerControl.LocalPlayer == Player);

            if (Player.Data.IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Uninvis()
        {
            Enabled = false;
            LastInvis = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
        }
    }
}