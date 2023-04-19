using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using System.Linq;
using Hazel;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ambusher : IntruderRole
    {
        public bool Enabled;
        public DateTime LastAmbushed;
        public float TimeRemaining;
        public bool OnAmbush => TimeRemaining > 0f;
        public PlayerControl AmbushedPlayer;
        public PlayerControl ClosestAmbush;
        public CustomButton AmbushButton;

        public Ambusher(PlayerControl player) : base(player)
        {
            Name = "Ambusher";
            StartText = "Spook The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = $"- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Ambusher : Colors.Intruder;
            RoleType = RoleEnum.Ambusher;
            RoleAlignment = RoleAlignment.IntruderKill;
            AlignmentName = IK;
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Ambusher;
            AmbushedPlayer = null;
            AmbushButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", HitAmbush);
        }

        public float AmbushTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAmbushed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.AmbushCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Ambush()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || AmbushedPlayer.Data.IsDead || AmbushedPlayer.Data.Disconnected || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnAmbush()
        {
            Enabled = false;
            LastAmbushed = DateTime.UtcNow;
            AmbushedPlayer = null;
        }

        public void HitAmbush()
        {
            if (AmbushTimer() != 0f || Utils.IsTooFar(Player, ClosestAmbush) || ClosestAmbush == AmbushedPlayer)
                return;

            var interact = Utils.Interact(Player, ClosestAmbush);

            if (interact[3])
            {
                TimeRemaining = CustomGameOptions.AmbushDuration;
                AmbushedPlayer = ClosestAmbush;
                Ambush();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Ambush);
                writer.Write(Player.PlayerId);
                writer.Write(AmbushedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else if (interact[0])
                LastAmbushed = DateTime.UtcNow;
            else if (interact[1])
                LastAmbushed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notAmbushed = PlayerControl.AllPlayerControls.ToArray().Where(x => x != AmbushedPlayer).ToList();
            AmbushButton.Update("AMBUSH", AmbushTimer(), CustomGameOptions.AmbushDuration, notAmbushed, OnAmbush, TimeRemaining, CustomGameOptions.AmbushDuration);
        }
    }
}