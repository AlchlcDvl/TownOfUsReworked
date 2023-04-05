using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class Hide
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsEnded || ConstantVariables.IsLobby)
                return;

            foreach (var ghoul in Role.GetRoles<Ghoul>(RoleEnum.Ghoul))
            {
                if (ghoul.Player.Data.Disconnected)
                    continue;

                var caught = ghoul.Caught;

                if (!caught)
                    ghoul.Fade();
                else if (ghoul.Faded)
                {
                    Utils.DefaultOutfit(ghoul.Player);
                    ghoul.Player.MyRend().color = Color.white;
                    ghoul.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    ghoul.Faded = false;
                    ghoul.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}