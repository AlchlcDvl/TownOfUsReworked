using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.EnforcerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class BombUnBomb
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var enf in Role.GetRoles<Enforcer>(RoleEnum.Enforcer))
            {
                if (enf.DelayActive)
                    enf.Delay();
                else if (enf.Bombing)
                    enf.Boom();
                else if (enf.Enabled)
                    enf.Unboom();
            }
        }
    }
}