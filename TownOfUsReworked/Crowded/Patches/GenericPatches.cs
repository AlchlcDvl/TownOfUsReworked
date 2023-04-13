using TownOfUsReworked.Crowded.Net;
using HarmonyLib;
using Reactor.Networking.Rpc;

namespace TownOfUsReworked.Crowded.Patches
{
    [HarmonyPatch]
    public static class GenericPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckColor))]
        public static class PlayerControlCmdCheckColorPatch
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte colorId)
            {
                Rpc<SetColorRpc>.Instance.Send(__instance, colorId);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.IsSelectedItemEquipped))]
        public static class PlayerTabIsSelectedItemEquippedPatch
        {
            public static bool Prefix(out bool __result)
            {
                __result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
        public static class PlayerTabUpdateAvailableColorsPatch
        {
            public static bool Prefix(PlayerTab __instance)
            {
                __instance.AvailableColors.Clear();

                for (var i = 0; i < Palette.PlayerColors.Count; i++)
                {
                    if(!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.CurrentOutfit.ColorId != i)
                        __instance.AvailableColors.Add(i);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public static class GameStartManagerUpdatePatch
        {
            private static string fixDummyCounterColor;

            public static void Prefix(GameStartManager __instance)
            {
                if (GameData.Instance != null && __instance.LastPlayerCount != GameData.Instance.PlayerCount)
                {
                    if (__instance.LastPlayerCount > __instance.MinPlayers)
                        fixDummyCounterColor = "00FF00FF";
                    else if (__instance.LastPlayerCount == __instance.MinPlayers)
                        fixDummyCounterColor = "FFFF00FF";
                    else
                        fixDummyCounterColor = "FF0000FF";
                }
            }

            public static void Postfix(GameStartManager __instance)
            {
                if (fixDummyCounterColor != null)
                {
                    __instance.PlayerCounter.text = $"<color=#{fixDummyCounterColor}>{GameData.Instance.PlayerCount}/{GameManager.Instance.LogicOptions.MaxPlayers}";
                    fixDummyCounterColor = null;
                }
            }
        }
    }
}