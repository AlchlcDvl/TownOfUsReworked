using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class LustUnlust
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var sk in Role.GetRoles<SerialKiller>(RoleEnum.SerialKiller))
            {
                if (sk.Lusted)
                    sk.Bloodlust();
                else if (sk.Enabled)
                    sk.Unbloodlust();
            }
        }
    }
}