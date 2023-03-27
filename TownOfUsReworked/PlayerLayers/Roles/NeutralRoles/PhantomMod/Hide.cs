using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class Hide
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsEnded || ConstantVariables.IsLobby)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Phantom))
            {
                var phantom = (Phantom)role;

                if (role.Player.Data.Disconnected)
                    continue;

                var caught = phantom.Caught;

                if (!caught)
                    phantom.Fade();
                else if (phantom.Faded)
                {
                    Utils.DefaultOutfit(phantom.Player);
                    phantom.Player.MyRend().color = Color.white;
                    phantom.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    phantom.Faded = false;
                    phantom.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}