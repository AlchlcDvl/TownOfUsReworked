using HarmonyLib;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(OverlayKillAnimation), nameof(OverlayKillAnimation.Initialize))]
    static class OverlayKillAnimationPatch
    {
        static int currentOutfitTypeCache = 0;

        [HarmonyPrefix]
        public static void Prefix(GameData.PlayerInfo kInfo, GameData.PlayerInfo vInfo)
        {
            PlayerControl playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == kInfo.PlayerId);
            currentOutfitTypeCache = (int)playerControl.CurrentOutfitType;

            if (!CustomGameOptions.AppearanceAnimation)
                playerControl.CurrentOutfitType = PlayerOutfitType.Default;
        }
        
        [HarmonyPostfix]
        public static void Postfix(GameData.PlayerInfo kInfo, GameData.PlayerInfo vInfo)
        {
            PlayerControl playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == kInfo.PlayerId);
            playerControl.CurrentOutfitType = (PlayerOutfitType)currentOutfitTypeCache;
        }
    }
}
