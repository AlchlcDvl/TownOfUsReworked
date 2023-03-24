using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class LustUnlust
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.SerialKiller))
            {
                var sk = (SerialKiller)role;

                if (sk.Lusted)
                    sk.Bloodlust();
                else if (sk.Enabled)
                    sk.Unbloodlust();
            }
        }
    }
}