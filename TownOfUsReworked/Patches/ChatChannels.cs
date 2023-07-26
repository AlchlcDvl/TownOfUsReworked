namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    public static class ChatChannels
    {
        public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
        {
            if (__instance != HUD.Chat)
                return true;

            var localPlayer = CustomPlayer.Local;

            if (localPlayer == null)
                return true;

            var sourcerole = Role.GetRole(sourcePlayer);
            var shouldSeeMessage = (sourcePlayer.IsOtherLover(localPlayer) && CustomGameOptions.LoversChat && sourcerole.CurrentChannel == ChatChannel.Lovers) ||
                (sourcePlayer.IsOtherRival(localPlayer) && CustomGameOptions.RivalsChat && sourcerole.CurrentChannel == ChatChannel.Rivals) || (sourcePlayer.IsOtherLink(localPlayer) &&
                CustomGameOptions.LinkedChat && sourcerole.CurrentChannel == ChatChannel.Linked);

            if (DateTime.UtcNow - MeetingStart.MeetingStartTime < TimeSpan.FromSeconds(1))
                return shouldSeeMessage;

            return (Meeting || LobbyBehaviour.Instance || localPlayer.Data.IsDead || sourcePlayer == localPlayer || sourcerole.CurrentChannel == ChatChannel.All ||
                shouldSeeMessage) && !(Meeting && CustomPlayer.Local.IsSilenced());
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingStart
    {
        public static DateTime MeetingStartTime = DateTime.MinValue;

        public static void Prefix() => MeetingStartTime = DateTime.UtcNow;
    }
}