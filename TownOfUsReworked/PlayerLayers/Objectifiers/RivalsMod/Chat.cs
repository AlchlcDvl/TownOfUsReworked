using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.RivalsMod
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    public static class Chat
    {
        public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
        {
            if (__instance != HudManager.Instance.Chat)
                return true;

            var localPlayer = PlayerControl.LocalPlayer;

            if (localPlayer == null)
                return true;

            return MeetingHud.Instance != null || LobbyBehaviour.Instance != null || localPlayer.Data.IsDead || localPlayer.Is(ObjectifierEnum.Rivals) || sourcePlayer ==
                PlayerControl.LocalPlayer;
        }
    }
}