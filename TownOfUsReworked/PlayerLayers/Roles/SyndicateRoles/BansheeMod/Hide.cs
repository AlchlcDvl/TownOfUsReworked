using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class Hide
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Banshee))
            {
                var banshee = (Banshee)role;

                if (role.Player.Data.Disconnected)
                    continue;

                var caught = banshee.Caught;

                if (!caught)
                    banshee.Fade();
                else if (banshee.Faded)
                {
                    Utils.DefaultOutfit(banshee.Player);
                    banshee.Player.MyRend().color = Color.white;
                    banshee.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    banshee.Faded = false;
                    banshee.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}