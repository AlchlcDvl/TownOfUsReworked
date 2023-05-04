using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
    public static class RecomputeTaskCounts
    {
        public static bool Prefix(GameData __instance)
        {
            __instance.TotalTasks = 0;
            __instance.CompletedTasks = 0;

            for (var i = 0; i < __instance.AllPlayers.Count; i++)
            {
                var playerInfo = __instance.AllPlayers.ToArray()[i];

                if (!playerInfo.Disconnected && playerInfo.Object != null && playerInfo.Tasks != null && playerInfo.Object.CanDoTasks() && !(playerInfo.IsDead &&
                    playerInfo.Object.Is(RoleEnum.Revealer)))
                {
                    for (var j = 0; j < playerInfo.Tasks.Count; j++)
                    {
                        __instance.TotalTasks++;

                        if (playerInfo.Tasks.ToArray()[j].Complete)
                            __instance.CompletedTasks++;
                    }
                }
            }

            return __instance.TotalTasks != 0;
        }
    }

    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public static class CanUse
    {
        public static bool Prefix(Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref float __result)
        {
            var playerControl = playerInfo.Object;

            var flag = !playerControl.CanDoTasks();

            //If the console is not a sabotage repair console
            if (flag && !__instance.AllowImpostor)
            {
                __result = float.MaxValue;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static class BeginShip
    {
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

    [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__114), nameof(PlayerControl._CoSetTasks_d__114.MoveNext))]
    public static class PlayerControl_SetTasks
    {
        public static void Postfix(PlayerControl._CoSetTasks_d__114 __instance)
        {
            if (__instance == null || ConstantVariables.IsHnS)
                return;

            __instance.__4__this.RegenTask();
        }
    }
}