using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    class SetTasks
    {
        public static void Postfix(PlayerControl __instance)
        {
            var role = Role.GetRole(__instance);
            var modifier = Modifier.GetModifier(__instance);
            if (role.Faction == Faction.Crewmates) return;
            if (role.Faction != Faction.Crewmates && role.RoleType != RoleEnum.Phantom) return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var totalTasks = taskinfos.Count();
        }
    }
}
