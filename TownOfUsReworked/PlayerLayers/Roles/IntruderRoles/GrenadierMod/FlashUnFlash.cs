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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var grenadier in Role.GetRoles<Grenadier>(RoleEnum.Grenadier))
            {
                if (grenadier.Flashed)
                    grenadier.Flash();
                else if (grenadier.Enabled)
                    grenadier.UnFlash();
            }
        }
    }
}