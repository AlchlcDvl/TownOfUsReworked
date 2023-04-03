using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class UpdateSpeed
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.Is(RoleEnum.Janitor))
            {
                var role = Role.GetRole<Janitor>(__instance.myPlayer);

                if (role.CurrentlyDragging != null && __instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                    __instance.body.velocity *= CustomGameOptions.DragModifier;
            }
        }
    }
}