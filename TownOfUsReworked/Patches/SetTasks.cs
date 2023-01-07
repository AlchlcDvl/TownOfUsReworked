using HarmonyLib;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    class SetTasks
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance == null)
                return;

            if (!__instance.CanDoTasks())
                return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var totalTasks = taskinfos.Count();
        }
    }
}
