using HarmonyLib;
using UnityEngine;
using AmongUs.Data;

namespace TownOfUsReworked.Patches
{
    //Thanks to Town Of Host for this code
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public class ChatControlPatch
    {
        public static void Prefix()
        {
            if (AmongUsClient.Instance.AmHost && DataManager.Settings.multiplayer.ChatMode == InnerNet.QuickChatModes.QuickChatOnly)
                DataManager.Settings.multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
        }

        public static void Postfix(ChatController __instance)
        {
            if (!__instance.TextArea.hasFocus)
                return;

            if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
                ClipboardHelper.PutClipboardString(__instance.TextArea.text);

            if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
                __instance.TextArea.SetText(__instance.TextArea.text + GUIUtility.systemCopyBuffer);

            if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.X))
            {
                ClipboardHelper.PutClipboardString(__instance.TextArea.text);
                __instance.TextArea.SetText("");
            }
        }
    }
}