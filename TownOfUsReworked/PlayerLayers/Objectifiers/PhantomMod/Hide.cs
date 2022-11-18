using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.PhantomMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var objectifier in Objectifier.GetObjectifiers(ObjectifierEnum.Phantom))
            {
                var phantom = (Phantom)objectifier;

                if (objectifier.Player.Data.Disconnected)
                    return;

                var caught = phantom.Caught;
                
                if (!caught)
                    phantom.Fade();
                else if (phantom.Faded)
                {
                    Utils.Unmorph(phantom.Player);
                    phantom.Player.myRend().color = Color.white;
                    phantom.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    phantom.Faded = false;
                    phantom.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}