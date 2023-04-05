using System;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Morphling : IntruderRole
    {
        public AbilityButton MorphButton;
        public AbilityButton SampleButton;
        public DateTime LastMorphed;
        public DateTime LastSampled;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public PlayerControl ClosestTarget;
        public float TimeRemaining;
        public bool Enabled;
        public bool Morphed => TimeRemaining > 0f;

        public Morphling(PlayerControl player) : base(player)
        {
            Name = "Morphling";
            StartText = "Fool The <color=#8BFDFDFF>Crew</color> With Your Appearances";
            AbilitiesText = $"- You can morph into other players to fool the <color=#8BFDFDFF>Crew</color>\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Morphling : Colors.Intruder;
            LastMorphed = DateTime.UtcNow;
            RoleType = RoleEnum.Morphling;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
            SampledPlayer = null;
            MorphedPlayer = null;
        }

        public void Morph()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);
            Enabled = true;

            if (Player.Data.IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Enabled = false;
            Player.RpcRevertShapeshift(true);
            Utils.DefaultOutfit(Player);
            LastMorphed = DateTime.UtcNow;

            if (CustomGameOptions.MorphCooldownsLinked)
                LastSampled = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMorphed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MorphlingCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float SampleTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSampled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.SampleCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}
