using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            if (GameStates.IsEnded || GameStates.IsLobby)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Ghoul))
            {
                var ghoul = (Ghoul)role;

                if (role.Player.Data.Disconnected)
                    continue;
                
                var caught = ghoul.Caught;

                if (!caught)
                    ghoul.Fade();
                else if (ghoul.Faded)
                {
                    Utils.DefaultOutfit(ghoul.Player);
                    ghoul.Player.myRend().color = Color.white;
                    ghoul.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    ghoul.Faded = false;
                    ghoul.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}