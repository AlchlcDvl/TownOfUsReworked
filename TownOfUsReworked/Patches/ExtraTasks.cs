using HarmonyLib;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ShipStatus))]
    public static class ExtraTasks
    {
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
        public static void Postfix(ref bool __result) => __result = false;

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool Prefix()
        {
            var commonTask = CustomGameOptions.CommonTasks;
            var normalTask = CustomGameOptions.ShortTasks;
            var longTask = CustomGameOptions.LongTasks;

            if (GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks > commonTask)
                GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = commonTask;

            if (GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks > normalTask)
                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = normalTask;

            if (GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks > longTask)
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = longTask;

            return true;
        }
    }
}