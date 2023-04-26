using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    public static class Chats
    {
        public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
        {
            if (__instance != HudManager.Instance.Chat)
                return true;

            var localPlayer = PlayerControl.LocalPlayer;

            if (localPlayer == null)
                return true;

            var localrole = Role.GetRole(localPlayer);
            var sourcerole = Role.GetRole(sourcePlayer);
            return MeetingHud.Instance != null || LobbyBehaviour.Instance != null || localPlayer.Data.IsDead || sourcePlayer == localPlayer || (sourcePlayer.GetOtherLover() == localPlayer
                && CustomGameOptions.LoversChat && sourcerole.CurrentChannel == ChatChannel.Lovers) || (sourcePlayer.GetOtherRival() == localPlayer && CustomGameOptions.RivalsChat &&
                sourcerole.CurrentChannel == ChatChannel.Rivals);
        }
    }
}