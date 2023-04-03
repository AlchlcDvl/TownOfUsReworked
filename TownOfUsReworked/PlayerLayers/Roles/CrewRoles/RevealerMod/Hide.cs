using HarmonyLib;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class Hide
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsEnded || ConstantVariables.IsLobby)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Revealer))
            {
                var haunter = (Revealer)role;

                if (role.Player.Data.Disconnected)
                    return;

                var caught = haunter.Caught;

                if (!caught)
                    haunter.Fade();
                else if (haunter.Faded)
                {
                    Utils.DefaultOutfit(haunter.Player);
                    haunter.Player.MyRend().color = Color.white;
                    haunter.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    haunter.Faded = false;
                    haunter.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}