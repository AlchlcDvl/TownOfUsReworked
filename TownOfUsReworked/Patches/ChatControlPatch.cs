using HarmonyLib;
using UnityEngine;
using AmongUs.Data;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    //Thanks to Town Of Host for all this code
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public class ChatControlPatch
    {
        public static int CurrentHistorySelection = -1;
        
        public static void Prefix()
        {
            if (AmongUsClient.Instance.AmHost && DataManager.Settings.multiplayer.ChatMode == InnerNet.QuickChatModes.QuickChatOnly)
                DataManager.Settings.multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
        }

        public static void Postfix(ChatController __instance)
        {
            if (!__instance.TextArea.hasFocus)
                return;

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
                ClipboardHelper.PutClipboardString(__instance.TextArea.text);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
                __instance.TextArea.SetText(__instance.TextArea.text + GUIUtility.systemCopyBuffer);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.X))
            {
                ClipboardHelper.PutClipboardString(__instance.TextArea.text);
                __instance.TextArea.SetText("");
            }
            
            if (Input.GetKeyDown(KeyCode.UpArrow) && ChatCommands.ChatHistory.Count > 0)
            {
                CurrentHistorySelection = Mathf.Clamp(--CurrentHistorySelection, 0, ChatCommands.ChatHistory.Count - 1);
                __instance.TextArea.SetText(ChatCommands.ChatHistory[CurrentHistorySelection]);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && ChatCommands.ChatHistory.Count > 0)
            {
                CurrentHistorySelection++;

                if (CurrentHistorySelection < ChatCommands.ChatHistory.Count)
                    __instance.TextArea.SetText(ChatCommands.ChatHistory[CurrentHistorySelection]);
                else
                    __instance.TextArea.SetText("");
            }
        }
    }
    
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    class ChatUpdatePatch
    {
        public static void Postfix(ChatController __instance)
        {
            if (!AmongUsClient.Instance.AmHost || TownOfUsReworked.MessagesToSend.Count < 1 || (TownOfUsReworked.MessagesToSend[0].Item2 ==
                byte.MaxValue && TownOfUsReworked.MessageWait.Value > __instance.TimeSinceLastMessage))
                return;

            var player = PlayerControl.AllPlayerControls.ToArray().OrderBy(x => x.PlayerId).Where(x => !x.Data.IsDead).FirstOrDefault();

            if (player == null)
                return;
                
            (string msg, byte sendTo) = TownOfUsReworked.MessagesToSend[0];
            TownOfUsReworked.MessagesToSend.RemoveAt(0);
            int clientId = sendTo == byte.MaxValue ? -1 : Utils.PlayerById(sendTo).GetClientId();

            if (clientId == -1)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, msg);

            var writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)RpcCalls.SendChat, SendOption.None, clientId);
            writer.Write(msg);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            __instance.TimeSinceLastMessage = 0f;
        }
    }
}