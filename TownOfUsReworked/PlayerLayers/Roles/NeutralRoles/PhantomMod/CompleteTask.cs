using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public static class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Phantom))
                return;

            var role = Role.GetRole<Phantom>(__instance);

            if (role.TasksLeft == CustomGameOptions.PhantomTasksRemaining && CustomGameOptions.PhantomPlayersAlerted)
                Utils.Flash(role.Color, "A <color=#45076AFF>Phantom</color> is nearly done with their tasks!");

            if (role.TasksDone && !role.Caught)
                role.CompletedTasks = true;
        }
    }
}