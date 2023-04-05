using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class BlockUnblock
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var role in Role.GetRoles<Escort>(RoleEnum.Escort))
            {
                if (role.Blocking)
                    role.Block();
                else if (role.Enabled)
                    role.UnBlock();
            }
        }
    }
}