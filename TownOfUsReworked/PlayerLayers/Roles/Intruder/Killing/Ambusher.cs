using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ambusher : IntruderRole
    {
        public bool Enabled;
        public DateTime LastAmbushed;
        public float TimeRemaining;
        public bool OnAmbush => TimeRemaining > 0f;
        public PlayerControl AmbushedPlayer;
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
            InspectorResults = InspectorResults.HindersOthers;
            Type = LayerEnum.Ambusher;
            AmbushedPlayer = null;
            AmbushButton = new(this, "Ambush", AbilityTypes.Direct, "Secondary", HitAmbush, Exception1);
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

            if (IsDead || AmbushedPlayer.Data.IsDead || AmbushedPlayer.Data.Disconnected || MeetingHud.Instance)
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
            if (AmbushTimer() != 0f || Utils.IsTooFar(Player, AmbushButton.TargetPlayer) || AmbushButton.TargetPlayer == AmbushedPlayer)
                return;

            var interact = Utils.Interact(Player, AmbushButton.TargetPlayer);

            if (interact[3])
            {
                TimeRemaining = CustomGameOptions.AmbushDuration;
                AmbushedPlayer = AmbushButton.TargetPlayer;
                Ambush();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Ambush);
                writer.Write(PlayerId);
                writer.Write(AmbushedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else if (interact[0])
                LastAmbushed = DateTime.UtcNow;
            else if (interact[1])
                LastAmbushed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !CustomGameOptions.AmbushMates) || (player.Is(SubFaction) &&
            SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            AmbushButton.Update("AMBUSH", AmbushTimer(), CustomGameOptions.AmbushDuration, OnAmbush, TimeRemaining, CustomGameOptions.AmbushDuration);
        }
    }
}