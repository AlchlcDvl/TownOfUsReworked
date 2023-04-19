using System;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Modifiers;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Disguiser : IntruderRole, IVisualAlteration
    {
        public CustomButton DisguiseButton;
        public CustomButton MeasureButton;
        public DateTime LastDisguised;
        public DateTime LastMeasured;
        public float TimeRemaining;
        public float TimeRemaining2;
        public PlayerControl MeasuredPlayer;
        public PlayerControl DisguisePlayer;
        public PlayerControl DisguisedPlayer;
        public PlayerControl ClosestTarget;
        public bool DelayActive => TimeRemaining2 > 0f;
        public bool Disguised => TimeRemaining > 0f;
        public bool Enabled;

        public Disguiser(PlayerControl player) : base(player)
        {
            Name = "Disguiser";
            StartText = "Disguise The <color=#8BFDFDFF>Crew</color> To Frame Them";
            AbilitiesText = $"- You can disguise a player into someone else's appearance\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Disguiser : Colors.Intruder;
            RoleType = RoleEnum.Disguiser;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = ID;
            MeasuredPlayer = null;
            Type = LayerEnum.Disguiser;
            DisguiseButton = new(this, AssetManager.Disguise, AbilityTypes.Direct, "Secondary", Disguise);
            MeasureButton = new(this, AssetManager.Measure, AbilityTypes.Direct, "Tertiary", Measure);
            DisguisePlayer = null;
            DisguisedPlayer = null;
        }

        public void Disguise()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(DisguisedPlayer, DisguisePlayer);
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
            var num = Player.GetModifiedCooldown(CustomGameOptions.DisguiseCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MeasureTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMeasured;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MeasureCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void HitDisguise()
        {
            if (DisguiseTimer() != 0f || Utils.IsTooFar(Player, ClosestTarget) || ClosestTarget == MeasuredPlayer || Disguised || DelayActive)
                return;

            var interact = Utils.Interact(Player, ClosestTarget);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Disguise);
                writer.Write(Player.PlayerId);
                writer.Write(MeasuredPlayer.PlayerId);
                writer.Write(ClosestTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.DisguiseDuration;
                TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                DisguisePlayer = MeasuredPlayer;
                DisguisedPlayer = ClosestTarget;
                Delay();

                if (CustomGameOptions.DisgCooldownsLinked)
                    LastMeasured = DateTime.UtcNow;
            }
            else if (interact[0])
            {
                LastDisguised = DateTime.UtcNow;

                if (CustomGameOptions.DisgCooldownsLinked)
                    LastMeasured = DateTime.UtcNow;
            }
            else if (interact[1])
            {
                LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);

                if (CustomGameOptions.DisgCooldownsLinked)
                    LastMeasured.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
        }

        public void Measure()
        {
            if (MeasureTimer() != 0f || Utils.IsTooFar(Player, ClosestTarget) || ClosestTarget == MeasuredPlayer)
                return;

            var interact = Utils.Interact(Player, ClosestTarget);

            if (interact[3])
                MeasuredPlayer = ClosestTarget;

            if (interact[0])
            {
                LastMeasured = DateTime.UtcNow;

                if (CustomGameOptions.DisgCooldownsLinked)
                    LastDisguised = DateTime.UtcNow;
            }
            else if (interact[1])
            {
                LastMeasured.AddSeconds(CustomGameOptions.ProtectKCReset);

                if (CustomGameOptions.DisgCooldownsLinked)
                    LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notMeasured = PlayerControl.AllPlayerControls.ToArray().Where(x => MeasuredPlayer != x).ToList();
            var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => ((x.Is(Faction.Intruder) && CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders) ||
                (!x.Is(Faction.Intruder) && CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders) || CustomGameOptions.DisguiseTarget == DisguiserTargets.Everyone) && x
                != MeasuredPlayer).ToList();
            DisguiseButton.Update("DISGUISE", DisguiseTimer(), CustomGameOptions.DisguiseCooldown, targets, DelayActive || Disguised, DelayActive ? TimeRemaining2 : TimeRemaining,
                DelayActive ? CustomGameOptions.TimeToDisguise : CustomGameOptions.DisguiseDuration, true, MeasuredPlayer != null);
            MeasureButton.Update("MEASURE", MeasureTimer(), CustomGameOptions.MeasureCooldown, notMeasured);
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (DisguisePlayer != null)
            {
                appearance = DisguisePlayer.GetDefaultAppearance();
                var alteration = Modifier.GetModifier(DisguisePlayer) as IVisualAlteration;
                alteration?.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }
    }
}
