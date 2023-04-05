using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.OverlordMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (__instance == null)
                return;

            var overlord2 = Objectifier.AllObjectifiers.Find(x => x.ObjectifierType == ObjectifierEnum.Overlord && ((Overlord)x).IsAlive);

            if (overlord2 == null)
                return;

            var overlord3 = (Overlord)overlord2;

            if (overlord3.IsAlive)
            {
                var ovmessage = "";

                if (MeetingPatches.MeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                    ovmessage = "This is the last meeting to find and kill the Overlord. Should you fail, you will all lose!";
                else if (MeetingPatches.MeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                    ovmessage = "There seems to be an Overlord bent on dominating the mission! Kill them before they are successful!";

                if (ovmessage != "")
                {
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, ovmessage);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable);
                    writer.Write(ovmessage);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}