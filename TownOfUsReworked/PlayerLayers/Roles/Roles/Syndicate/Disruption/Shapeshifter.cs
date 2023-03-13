using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

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
            AbilitiesText = "No one will know who they were";
            Color = CustomGameOptions.CustomSynColors ? Colors.Shapeshifter : Colors.Syndicate;
            RoleType = RoleEnum.Shapeshifter;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
        }

        public void Shapeshift()
        {
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;

            if (!Enabled)
            {
                var allPlayers = PlayerControl.AllPlayerControls;

                foreach (var player in allPlayers)
                {
                    var random = UnityEngine.Random.RandomRangeInt(0, allPlayers.Count);

                    while (player == allPlayers[random])
                        random = UnityEngine.Random.RandomRangeInt(0, allPlayers.Count);

                    var otherPlayer = allPlayers[random];
                    Utils.Morph(player, otherPlayer);
                }

                Enabled = true;
            }
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
            var timeSpan = utcNow - LastShapeshifted;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}