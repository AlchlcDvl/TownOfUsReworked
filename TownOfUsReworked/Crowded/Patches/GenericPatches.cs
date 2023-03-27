using System.Linq;
using TownOfUsReworked.Crowded.Net;
using HarmonyLib;
using Reactor.Networking.Rpc;
using AmongUs.GameOptions;

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

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.AreInvalid))]
        public static class InvalidOptionsPatches
        {
            public static bool Prefix(GameOptionsData __instance, [HarmonyArgument(0)] int maxExpectedPlayers) => __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                || __instance.NumImpostors + 1 > maxExpectedPlayers / 2 || __instance.KillDistance is < 0 or > 2 || __instance.PlayerSpeedMod is <= 0f or > 3f;
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
                        fixDummyCounterColor = "<color=#00FF00FF>";
                    else if (__instance.LastPlayerCount == __instance.MinPlayers)
                        fixDummyCounterColor = "<color=#FFFF00FF>";
                    else
                        fixDummyCounterColor = "<color=#FF0000FF>";
                }
            }

            public static void Postfix(GameStartManager __instance)
            {
                if (fixDummyCounterColor != null)
                {
                    __instance.PlayerCounter.text = $"{fixDummyCounterColor}{GameData.Instance.PlayerCount}/{GameManager.Instance.LogicOptions.MaxPlayers}";
                    fixDummyCounterColor = null;
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public static class GameOptionsMenu_Start
        {
            public static void Postfix(ref GameOptionsMenu __instance) => __instance.GetComponentsInChildren<NumberOption>().First(o => o.Title ==
                StringNames.GameNumImpostors).ValidRange = new FloatRange(1, TownOfUsReworked.MaxImpostors);
        }
    }
}