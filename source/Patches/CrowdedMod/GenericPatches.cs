using System.Linq;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace CrowdedMod.Patches {
    internal static class GenericPatches {
        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.IsSelectedItemEquipped))]
        public static class PlayerTabIsSelectedItemEquippedPatch {
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
                    {
                        __instance.AvailableColors.Add(i);
                    }
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
                    {
                        fixDummyCounterColor = "<color=#00FF00FF>";
                    } else if (__instance.LastPlayerCount == __instance.MinPlayers)
                    {
                        fixDummyCounterColor = "<color=#FFFF00FF>";
                    }
                    else
                    {
                        fixDummyCounterColor = "<color=#FF0000FF>";
                    }
                }
            }

            public static void Postfix(GameStartManager __instance)
            {
                if (fixDummyCounterColor != null)
                {
                    __instance.PlayerCounter.text = $"{fixDummyCounterColor}{GameData.Instance.PlayerCount}/{PlayerControl.GameOptions.MaxPlayers}";
                    fixDummyCounterColor = null;
                }
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
        public static class GameSettingMenu_OnEnable // Credits to https://github.com/Galster-dev/GameSettingsUnlocker
        {
            static void Prefix(ref GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public static class GameOptionsMenu_Start
        {
            static void Postfix(ref GameOptionsMenu __instance)
            {
                __instance.GetComponentsInChildren<NumberOption>()
                    .First(o => o.Title == StringNames.GameNumImpostors)
                    .ValidRange = new FloatRange(1, (int)(CreateGameOptionsPatches.CreateOptionsPicker_Start.maxPlayers-0.5f)/2);
            }
        }
    }
}
