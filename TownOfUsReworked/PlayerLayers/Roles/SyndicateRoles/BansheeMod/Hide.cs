using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            if (GameStates.IsEnded || GameStates.IsLobby)
                return;

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
                    banshee.Player.myRend().color = Color.white;
                    banshee.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    banshee.Faded = false;
                    banshee.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}