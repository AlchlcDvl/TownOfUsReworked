using HarmonyLib;
using Hazel;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    public class Reportmessage
    {
        public static string location;

        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public class Recordlocation
        {
            [HarmonyPostfix]
            public static void postfix(RoomTracker __instance)
            {
                if (__instance.text.transform.localPosition.y != -3.25f)
                    location = __instance.text.text;
                else
                {
                    string name = PlayerControl.LocalPlayer.name;
                    location = $"a hallway/outside, {name} where is the body?";
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
        public class Sendchat
        {
            [HarmonyPostfix]
            public static void postfix([HarmonyArgument(0)] GameData.PlayerInfo target)
            {
                string report = $"The body was found in {location}.";

                if (target != null && CustomGameOptions.LocationReports)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Sendchat,
                        SendOption.Reliable, -1);
                    writer.Write(report);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}