using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class VisibleOverride
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
        {
            if (!__instance.Is(ObjectifierEnum.Phantom))
                return;
            
            if (!Objectifier.GetObjectifier<Phantom>(__instance).HasDied)
                return;

            if (Objectifier.GetObjectifier<Phantom>(__instance).Caught)
                return;

            value = !__instance.inVent;
        }
    }
}