using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Survivor : NeutralRole
    {
        public bool Enabled;
        public DateTime LastVested;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Vesting => TimeRemaining > 0f;
        public bool Alive => !Player.Data.Disconnected && !Player.Data.IsDead;
        public CustomButton VestButton;

        public Survivor(PlayerControl player) : base(player)
        {
            Name = "Survivor";
            StartText = "Do Whatever It Takes To Live";
            AbilitiesText = "- You can put on a vest, which makes you unkillable for a short duration of time";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Survivor : Colors.Neutral;
            RoleType = RoleEnum.Survivor;
            UsesLeft = CustomGameOptions.MaxVests;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            Objectives = "- Live to the end of the game";
            InspectorResults = InspectorResults.SeeksToProtect;
            Type = LayerEnum.Survivor;
            VestButton = new(this, AssetManager.Vest, AbilityTypes.Effect, "ActionSecondary", HitVest, true);
        }

        public float VestTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastVested;
            var num = Player.GetModifiedCooldown(CustomGameOptions.VestCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Vest()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnVest()
        {
            Enabled = false;
            LastVested = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            VestButton.Update("PROTECT", VestTimer(), CustomGameOptions.VestCd, UsesLeft, Vesting, TimeRemaining, CustomGameOptions.VestDuration);
        }

        public void HitVest()
        {
            if (!ButtonUsable || VestTimer() != 0f || Vesting)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Vest);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.VestDuration;
            UsesLeft--;
            Vest();
        }
    }
}