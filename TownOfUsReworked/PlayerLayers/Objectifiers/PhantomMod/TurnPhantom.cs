using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class TurnPhantom
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;

            if (!__instance.Is(ObjectifierEnum.Phantom))
                return true;

            var phantom = Objectifier.GetObjectifier<Phantom>(__instance);

            if (reason == DeathReason.Exile || reason == DeathReason.Kill)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                phantom.HasDied = true;
            }

            return true;
        }
    }
}