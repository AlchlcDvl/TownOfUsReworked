using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
    public class EnableMapImps
    {
        private static void Prefix(ref GameSettingMenu __instance)
        {
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }
}