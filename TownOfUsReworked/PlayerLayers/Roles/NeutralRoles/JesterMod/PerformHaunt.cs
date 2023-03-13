using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformHaunt
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jester))
                return true;

            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);

            if (__instance == role.HauntButton)
            {
                if (!Utils.ButtonUsable(role.HauntButton))
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.HauntTimer() != 0f)
                    return false;
                    
                Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer, false);
                role.HasHaunted = true;
                role.MaxUses--;
                role.LastHaunted = DateTime.UtcNow;
                return false;
            }

            return true;
        }
    }
}