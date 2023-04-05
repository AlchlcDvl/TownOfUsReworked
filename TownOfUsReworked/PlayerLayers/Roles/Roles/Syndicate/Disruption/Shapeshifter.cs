using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Shapeshifter : SyndicateRole
    {
        public AbilityButton ShapeshiftButton;
        public bool Enabled;
        public DateTime LastShapeshifted;
        public float TimeRemaining;
        public bool Shapeshifted => TimeRemaining > 0f;

        public Shapeshifter(PlayerControl player) : base(player)
        {
            Name = "Shapeshifter";
            StartText = "Change Everyone's Appearances";
            AbilitiesText = $"- You can shuffle everyone's appearances\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Shapeshifter : Colors.Syndicate;
            RoleType = RoleEnum.Shapeshifter;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
        }

        public void Shapeshift()
        {
            TimeRemaining -= Time.deltaTime;
            Enabled = true;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnShapeshift()
        {
            Enabled = false;
            LastShapeshifted = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float ShapeshiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastShapeshifted;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}