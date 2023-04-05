using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class DisguiseUndisguise
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var disguiser in Role.GetRoles<Disguiser>(RoleEnum.Disguiser))
            {
                if (disguiser.DelayActive)
                    disguiser.Delay();
                else if (disguiser.Disguised)
                    disguiser.Disguise();
                else if (disguiser.Enabled)
                    disguiser.UnDisguise();
            }
        }
    }
}