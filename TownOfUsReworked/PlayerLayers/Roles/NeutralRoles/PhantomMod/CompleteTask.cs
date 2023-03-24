using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Reactor.Utilities;

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
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == PlayerControl.LocalPlayer)
                        Coroutines.Start(Utils.FlashCoroutine(role.Color));
                }
            }

            if (role.TasksDone && !role.Caught)
                role.CompletedTasks = true;
        }
    }
}