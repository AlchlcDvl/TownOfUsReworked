using HarmonyLib;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class ChaosDriveCount
    {
        public static bool messageSent = false;

        public static void Postfix()
        {
            Role.ChaosDriveMeetingTimerCount += 1;

            if (Role.ChaosDriveMeetingTimerCount >= CustomGameOptions.ChaosDriveMeetingCount && !Role.SyndicateHasChaosDrive)
                Role.SyndicateHasChaosDrive = true;

            var message = "";
            
            if (!Role.SyndicateHasChaosDrive)
            {
                if (Role.ChaosDriveMeetingTimerCount == CustomGameOptions.ChaosDriveMeetingCount - 1)
                    message = "This is the last meeting before the Syndicate gets their hands on the Chaos Drive!";
                else
                    message = $"{CustomGameOptions.ChaosDriveMeetingCount - Role.ChaosDriveMeetingTimerCount} remain till the Syndicate gets their hands on the Chaos Drive!";
            }
            else
                message = "The Syndicate now possesses the Chaos Drive!";

            if (!messageSent)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable, -1);
                writer.Write(message);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                messageSent = Role.SyndicateHasChaosDrive;
            }
        }
    }
}