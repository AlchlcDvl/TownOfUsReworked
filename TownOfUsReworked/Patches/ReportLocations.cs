using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Patches
{
    //Thanks to twix for this code
    public class Reportmessage
    {
        public static string location;

        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public class Recordlocation
        {
            [HarmonyPostfix]
            public static void Postfix(RoomTracker __instance)
            {
                if (__instance.text.transform.localPosition.y != -3.25f)
                    location = __instance.text.text;
                else
                {
                    string name = PlayerControl.LocalPlayer.name;
                    location = $"a hallway or somewhere outside, {name} where is the body?";
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
        public class Sendchat
        {
            [HarmonyPostfix]
            public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo target)
            {
                if (target != null && CustomGameOptions.LocationReports)
                {
                    string report = $"The body was found in {location}.";
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable, -1);
                    writer.Write(report);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}