using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.RevealerMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.myPlayer.Is(AbilityEnum.Revealer))
                return;

            var role = Ability.GetAbility<Revealer>(__instance.myPlayer);
            __instance.myPlayer.Collider.enabled = !role.Caught;
        }
    }
}