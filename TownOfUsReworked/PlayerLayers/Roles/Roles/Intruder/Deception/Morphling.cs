using System;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

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
            StartText = "Transform Into <color=#8BFDFDFF>Crewmates</color> to frame them";
            AbilitiesText = "Morph into <color=#8BFDFD>Crewmates</color> to frame them";
            Color = CustomGameOptions.CustomIntColors ? Colors.Morphling : Colors.Intruder;
            LastMorphed = DateTime.UtcNow;
            RoleType = RoleEnum.Morphling;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
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
            var timeSpan = utcNow - LastMorphed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MorphlingCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float SampleTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSampled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.SampleCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
