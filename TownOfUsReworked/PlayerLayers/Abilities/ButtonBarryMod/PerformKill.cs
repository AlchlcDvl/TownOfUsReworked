using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.ButtonBarryMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.ButtonBarry))
                return true;

            var role = Ability.GetAbility<ButtonBarry>(PlayerControl.LocalPlayer);

            if (__instance != role.ButtonButton)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead || role.ButtonUsed || PlayerControl.LocalPlayer.RemainingEmergencies <= 0 || !__instance.enabled)
                return false;

            role.ButtonUsed = true;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.BarryButton);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            if (AmongUsClient.Instance.AmHost)
            {
                MeetingRoomManager.Instance.reporter = PlayerControl.LocalPlayer;
                MeetingRoomManager.Instance.target = null;
                AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());

                if (Utils.TasksDone())
                    return false;

                DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.RpcStartMeeting(null);
            }

            return false;
        }
    }
}