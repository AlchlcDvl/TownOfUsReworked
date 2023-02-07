using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        private static bool hasFlashed = false;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Phantom))
                return;

            var role = Role.GetRole<Phantom>(__instance);

            if (role.TasksLeft == CustomGameOptions.PhantomTasksRemaining && !hasFlashed && CustomGameOptions.PhantomPlayersAlerted)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == PlayerControl.LocalPlayer)
                        Coroutines.Start(Utils.FlashCoroutine(role.Color));
                }

                hasFlashed = true;
            }

            if (role.TasksDone && !role.Caught)
                role.CompletedTasks = true;
        }
    }
}