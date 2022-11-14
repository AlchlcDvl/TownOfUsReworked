using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Abilities.RevealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Ability.GetAbilities(AbilityEnum.Revealer))
            {
                var haunter = (Revealer) role;

                if (role.Player.Data.Disconnected)
                    return;

                var caught = haunter.Caught;

                if (!caught)
                    haunter.Fade();
                else if (haunter.Faded)
                {
                    Utils.Unmorph(haunter.Player);
                    haunter.Player.myRend().color = Colors.Revealer;
                    haunter.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    haunter.Faded = false;
                    haunter.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}