using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Random = UnityEngine.Random;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Camouflager : IntruderRole
    {
        public AbilityButton CamouflageButton;
        public bool Enabled = false;
        public DateTime LastCamouflaged;
        public float TimeRemaining;
        public bool Camouflaged => TimeRemaining > 0f;

        public Camouflager(PlayerControl player) : base(player)
        {
            Name = "Camouflager";
            StartText = "Hinder The <color=#8BFDFDFF>Crew</color>'s Recognition";
            AbilitiesText = "- You can disrupt everyone's vision, causing them to be unable to tell players apart.\n- When camouflaged, everyone will appear grey with no name or " +
                "cosmetics." + (CustomGameOptions.MeetingColourblind ? "\n- This effect carries over into the meeting if a meeting is called during a camouflage." : "");
            Color = CustomGameOptions.CustomIntColors ? Colors.Camouflager : Colors.Intruder;
            RoleType = RoleEnum.Camouflager;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = IC;
            InspectorResults = InspectorResults.BringsChaos;
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.CamouflagerCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}