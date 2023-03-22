using HarmonyLib;
using AmongUs.Data.Player;
using AmongUs.Data.Legacy;

namespace TownOfUsReworked.Patches
{
    class SaveManager
    {
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.FileName), MethodType.Getter)]
        public class SaveManagerPatch
        {
            public static void Postfix(ref string __result) => __result += "_ToU-Rew";
        }

        [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.GetPrefsName))]
        public class LegacySaveManagerPatch
        {
            public static void Postfix(ref string __result) => __result += "_ToU-Rew";
        }
    }
}