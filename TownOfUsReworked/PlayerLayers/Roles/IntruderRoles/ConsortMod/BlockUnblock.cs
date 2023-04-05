using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ConsortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class BlockUnblock
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var consort in Role.GetRoles<Consort>(RoleEnum.Consort))
            {
                if (consort.Blocking)
                    consort.Block();
                else if (consort.Enabled)
                    consort.UnBlock();
            }
        }
    }
}