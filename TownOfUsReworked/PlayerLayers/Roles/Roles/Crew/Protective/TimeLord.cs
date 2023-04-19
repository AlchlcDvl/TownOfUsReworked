using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Functions;
using Hazel;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class TimeLord : CrewRole
    {
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public DateTime StartRewind;
        public DateTime FinishRewind;
        public CustomButton RewindButton;

        public TimeLord(PlayerControl player) : base(player)
        {
            Name = "Time Lord";
            StartText = "Rewind Time To Harass The <color=#FF0000FF>Evildoers</color>";
            AbilitiesText = "- You can rewind time, which will force players to move back to a previous location" + (CustomGameOptions.RewindRevive ? "\n- Rewinding time will also" +
                $" revive anyone who has died in the last {CustomGameOptions.RewindDuration}s" : "");
            Color = CustomGameOptions.CustomCrewColors ? Colors.TimeLord : Colors.Crew;
            RoleType = RoleEnum.TimeLord;
            UsesLeft = CustomGameOptions.RewindMaxUses;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = CP;
            InspectorResults = InspectorResults.DifferentLens;
            RoleBlockImmune = true;
            Type = LayerEnum.TimeLord;
            RewindButton = new(this, AssetManager.Rewind, AbilityTypes.Effect, "ActionSecondary", Rewind, true);
        }

        public static float GetCooldown() => RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomGameOptions.RewindCooldown;

        public float TimeLordRewindTimer()
        {
            var utcNow = DateTime.UtcNow;
            var num = (RecordRewind.rewinding ? CustomGameOptions.RewindDuration : Player.GetModifiedCooldown(CustomGameOptions.RewindCooldown)) * 1000f / (RecordRewind.rewinding ?
                3f : 1f);
            var timespan = utcNow - (RecordRewind.rewinding ? StartRewind : FinishRewind);
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Rewind()
        {
            if (TimeLordRewindTimer() != 0f && !RecordRewind.rewinding)
                return;

            UsesLeft--;
            StartStop.StartRewind(this);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Rewind);
            writer.Write(Player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RewindButton.Update("REWIND", TimeLordRewindTimer(), GetCooldown(), UsesLeft, ButtonUsable, ButtonUsable);
        }
    }
}