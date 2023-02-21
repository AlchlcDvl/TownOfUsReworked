using HarmonyLib;
using TownOfUsReworked.Enums;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using Hazel;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.OverlordMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (__instance == null)
                return;

            var overlord2 = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Overlord && ((Overlord)x).IsAlive && !((Overlord)x).OverlordWins);

            if (overlord2 == null)
                return;

            foreach (var ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord))
            {
                var overlord = (Overlord)ov;

                if (overlord.IsAlive && !overlord.OverlordWins)
                    overlord.OverlordMeetingCount += 1;
            }

            var overlord3 = (Overlord)overlord2;

            if (overlord3.IsAlive)
            {
                var ovmessage = "";

                if (overlord3.OverlordMeetingCount == CustomGameOptions.OverlordMeetingWinCount - 1)
                    ovmessage = "This is the last meeting to find and kill the Overlord. Should you fail, you will all lose!";
                else if (overlord3.OverlordMeetingCount < CustomGameOptions.OverlordMeetingWinCount - 1)
                    ovmessage = "There seems to be an Overlord bent on dominating the mission! Kill them before they are successful!";

                if (ovmessage != "")
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, ovmessage);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SendChat, SendOption.Reliable, -1);
                    writer.Write(ovmessage);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}