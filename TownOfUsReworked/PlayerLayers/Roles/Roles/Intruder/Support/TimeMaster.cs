using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Data;
using Hazel;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class TimeMaster : IntruderRole
    {
        public CustomButton FreezeButton;
        public bool Enabled;
        public float TimeRemaining;
        public DateTime LastFrozen;
        public bool Frozen => TimeRemaining > 0f;

        public TimeMaster(PlayerControl player) : base(player)
        {
            Name = "Time Master";
            StartText = "Freeze Time To Stop The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = $"- You can freeze time to stop the <color=#8BFDFDFF>Crew</color> from moving\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.TimeMaster : Colors.Intruder;
            RoleType = RoleEnum.TimeMaster;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
            Type = LayerEnum.TimeMaster;
            FreezeButton = new(this, AssetManager.TimeFreeze, AbilityTypes.Effect, "Secondary", HitFreeze);
        }

        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFrozen;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FreezeCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TimeFreeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void Unfreeze()
        {
            Enabled = false;
            LastFrozen = DateTime.UtcNow;
            Freeze.UnfreezeAll();
        }

        public void HitFreeze()
        {
            if (FreezeTimer() != 0f || Frozen)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.TimeFreeze);
            writer.Write(Player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.FreezeDuration;
            Freeze.FreezeAll();
            TimeFreeze();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            FreezeButton.Update("TIME FREEZE", FreezeTimer(), CustomGameOptions.FreezeCooldown, Frozen, TimeRemaining, CustomGameOptions.FreezeDuration);
        }
    }
}