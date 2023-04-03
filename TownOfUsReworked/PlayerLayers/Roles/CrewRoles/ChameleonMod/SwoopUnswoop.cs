using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class SwoopUnSwoop
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Chameleon))
            {
                var chameleon = (Chameleon)role;

                if (chameleon.IsSwooped)
                    chameleon.Invis();
                else if (chameleon.Enabled)
                    chameleon.Uninvis();
            }
        }
    }
}