using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformMurder
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jester))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);

            if (__instance == role.HauntButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.HauntTimer() != 0f)
                    return false;
                    
                Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer, false);
                role.HasHaunted = true;
                role.MaxUses--;
                return false;
            }

            return false;
        }
    }
}