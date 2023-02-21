using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ChatUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (HudManager.Instance != null && HudManager.Instance.Chat != null)
            {
                foreach (var bubble in HudManager.Instance.Chat.chatBubPool.activeChildren)
                {
                    if (bubble.Cast<ChatBubble>().NameText != null && PlayerControl.LocalPlayer.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                    {
                        var role = Role.GetRole(PlayerControl.LocalPlayer);

                        if (role != null)
                            bubble.Cast<ChatBubble>().NameText.color = role.Color;
                    }
                }
            }
        }
    }
}