namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    public static class ChatChannels
    {
        public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
        {
            if (__instance != HudManager.Instance.Chat)
                return true;

            var localPlayer = PlayerControl.LocalPlayer;

            if (localPlayer == null)
                return true;

            var sourcerole = Role.GetRole(sourcePlayer);
            return (MeetingHud.Instance || LobbyBehaviour.Instance || localPlayer.Data.IsDead || sourcePlayer == localPlayer || (sourcePlayer.GetOtherLover() == localPlayer
                && CustomGameOptions.LoversChat && sourcerole.CurrentChannel == ChatChannel.Lovers) || (sourcePlayer.GetOtherRival() == localPlayer && CustomGameOptions.RivalsChat &&
                sourcerole.CurrentChannel == ChatChannel.Rivals) || sourcerole.CurrentChannel == ChatChannel.All) && !(MeetingHud.Instance && PlayerControl.LocalPlayer.IsSilenced());
        }
    }
}