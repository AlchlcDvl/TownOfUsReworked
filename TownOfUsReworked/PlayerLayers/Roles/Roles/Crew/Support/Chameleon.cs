using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Chameleon : CrewRole
    {
        public bool Enabled;
        public DateTime LastSwooped;
        public float TimeRemaining;
        public bool IsSwooped => TimeRemaining > 0f;
        public AbilityButton SwoopButton;

        public Chameleon(PlayerControl player) : base(player)
        {
            Name = "Chameleon";
            StartText = "Go Invisible To Stalk Players";
            AbilitiesText = "- You can turn invisible";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Chameleon : Colors.Crew;
            LastSwooped = DateTime.UtcNow;
            RoleType = RoleEnum.Chameleon;
            AlignmentName = CS;
            InspectorResults = InspectorResults.Unseen;
        }

        public float SwoopTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSwooped;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.SwoopCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public override void Effect()
        {
            if (IsSwooped)
                Invis();
            else if (Enabled)
                Uninvis();
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Invis(Player, PlayerControl.LocalPlayer == Player);

            if (MeetingHud.Instance || Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Uninvis()
        {
            Enabled = false;
            LastSwooped = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
        }
    }
}