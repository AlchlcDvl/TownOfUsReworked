using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities.ButtonBarryMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformButton
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, AbilityEnum.ButtonBarry))
                return true;

            var ability = Ability.GetAbility<ButtonBarry>(PlayerControl.LocalPlayer);

            if (__instance == ability.ButtonButton)
            {
                if (ability.ButtonUsed || PlayerControl.LocalPlayer.RemainingEmergencies <= 0)
                    return false;

                if (ability.StartTimer() != 0f)
                    return false;

                ability.ButtonUsed = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.BarryButton);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (AmongUsClient.Instance.AmHost)
                {
                    MeetingRoomManager.Instance.reporter = PlayerControl.LocalPlayer;
                    MeetingRoomManager.Instance.target = null;
                    AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                    HudManager.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
                    PlayerControl.LocalPlayer.RpcStartMeeting(null);
                }

                return false;
            }

            return true;
        }
    }
}