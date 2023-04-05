using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class Hide
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var banshee in Role.GetRoles<Banshee>(RoleEnum.Banshee))
            {
                if (banshee.Player.Data.Disconnected)
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

                if (banshee.Screaming)
                    banshee.Scream();
                else if (banshee.Enabled)
                    banshee.UnScream();
            }
        }
    }
}