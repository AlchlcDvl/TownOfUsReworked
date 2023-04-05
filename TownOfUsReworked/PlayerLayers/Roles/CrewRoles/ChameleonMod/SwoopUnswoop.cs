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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var role in Role.GetRoles<Chameleon>(RoleEnum.Chameleon))
            {
                if (role.IsSwooped)
                    role.Invis();
                else if (role.Enabled)
                    role.Uninvis();
            }
        }
    }
}