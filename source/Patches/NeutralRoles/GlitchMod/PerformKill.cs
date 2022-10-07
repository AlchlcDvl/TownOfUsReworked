using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    internal class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch) && __instance.isActiveAndEnabled &&
                !__instance.isCoolingDown)
                return Role.GetRole<Glitch>(PlayerControl.LocalPlayer).UseAbility(__instance);

            return true;
        }
    }
}