using System;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Modifiers;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Morphling : IntruderRole, IVisualAlteration
    {
        public CustomButton MorphButton;
        public CustomButton SampleButton;
        public DateTime LastMorphed;
        public DateTime LastSampled;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public float TimeRemaining;
        public bool Enabled;
        public bool Morphed => TimeRemaining > 0f;

        public Morphling(PlayerControl player) : base(player)
        {
            Name = "Morphling";
            StartText = "Fool The <color=#8BFDFDFF>Crew</color> With Your Appearances";
            AbilitiesText = $"- You can morph into other players, taking up their appearances as your own\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Morphling : Colors.Intruder;
            RoleType = RoleEnum.Morphling;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
            SampledPlayer = null;
            MorphedPlayer = null;
            Type = LayerEnum.Morphling;
            MorphButton = new(this, "Morph", AbilityTypes.Effect, "Secondary", HitMorph);
            SampleButton = new(this, "Sample", AbilityTypes.Direct, "Tertiary", Sample, Exception1);
            InspectorResults = InspectorResults.CreatesConfusion;
        }

        public void Morph()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);
            Enabled = true;

            if (IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Enabled = false;
            Utils.DefaultOutfit(Player);
            LastMorphed = DateTime.UtcNow;

            if (CustomGameOptions.MorphCooldownsLinked)
                LastSampled = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMorphed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MorphlingCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float SampleTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSampled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SampleCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (MorphedPlayer != null)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var alteration = Modifier.GetModifier(MorphedPlayer) as IVisualAlteration;
                alteration?.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public void HitMorph()
        {
            if (MorphTimer() != 0f || SampledPlayer == null || Morphed)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Morph);
            writer.Write(PlayerId);
            writer.Write(SampledPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.MorphlingDuration;
            MorphedPlayer = SampledPlayer;
            Morph();
        }

        public void Sample()
        {
            if (SampleTimer() != 0f || Utils.IsTooFar(Player, SampleButton.TargetPlayer) || SampledPlayer == SampleButton.TargetPlayer)
                return;

            var interact = Utils.Interact(Player, SampleButton.TargetPlayer);

            if (interact[3])
                SampledPlayer = SampleButton.TargetPlayer;

            if (interact[0])
            {
                LastSampled = DateTime.UtcNow;

                if (CustomGameOptions.MorphCooldownsLinked)
                    LastMorphed = DateTime.UtcNow;
            }
            else if (interact[1])
            {
                LastSampled.AddSeconds(CustomGameOptions.ProtectKCReset);

                if (CustomGameOptions.MorphCooldownsLinked)
                    LastMorphed.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
        }

        public bool Exception1(PlayerControl player) => player == SampledPlayer;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            MorphButton.Update("MORPH", MorphTimer(), CustomGameOptions.MorphlingCd, Morphed, TimeRemaining, CustomGameOptions.MorphlingDuration, true, SampledPlayer != null);
            SampleButton.Update("SAMPLE", SampleTimer(), CustomGameOptions.MeasureCooldown);
        }
    }
}
