using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Classes;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class ButtonBarry : Ability
    {
        public bool ButtonUsed;
        public CustomButton ButtonButton;
        public DateTime LastButtoned;
        public bool ButtonUsable => !ButtonUsed && Player.RemainingEmergencies > 0;

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = "- You can call a button from anywhere";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.ButtonBarry : Colors.Ability;
            AbilityType = AbilityEnum.ButtonBarry;
            Type = LayerEnum.ButtonBarry;
            ButtonButton = new(this, "Button", AbilityTypes.Effect, "Quarternary", Call, true);
        }

        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastButtoned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ButtonCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Call()
        {
            if (!ButtonUsable || StartTimer() != 0f)
                return;

            ButtonUsed = true;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.BarryButton);
            writer.Write(Player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            if (AmongUsClient.Instance.AmHost)
            {
                MeetingRoomManager.Instance.reporter = Player;
                MeetingRoomManager.Instance.target = null;
                AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                HudManager.Instance.OpenMeetingRoom(Player);
                Player.RpcStartMeeting(null);
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ButtonButton.Update("BUTTON", StartTimer(), CustomGameOptions.ButtonCooldown, Player.RemainingEmergencies, ButtonUsable);
        }
    }
}