using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GrenadierMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class FlashUnFlash
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Grenadier))
            {
                var grenadier = (Grenadier)role;

                if (grenadier.Flashed)
                    grenadier.Flash();
                else if (grenadier.Enabled)
                    grenadier.UnFlash();
            }
        }
    }
}