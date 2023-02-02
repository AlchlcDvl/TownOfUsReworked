using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    //Vent and kill shit
    //Yes thank you Discussions - AD
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ToggleHighlight))]
    class ToggleHighlightPatch
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] bool active, [HarmonyArgument(1)] RoleTeamTypes team)
        {
            var player = PlayerControl.LocalPlayer;
            bool isActive = Utils.CanInteract(player);

            if (isActive)
            {
                var role = Role.GetRole(player); 
                var color = role != null ? role.Color : new Color32(255, 255, 255, 255);
                ((Renderer)__instance.cosmetics.currentBodySprite.BodySprite).material.SetColor("_OutlineColor", color);
            }
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
    class SetVentOutlinePatch
    {
        public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool mainTarget)
        {
            var player = PlayerControl.LocalPlayer;
            bool active = player != null && !MeetingHud.Instance && Utils.CanVent(player, player.Data);

            if (active)
            {
                var role = Role.GetRole(player); 
                var color = role != null ? role.Color : new Color32(255, 255, 255, 255);
                ((Renderer)__instance.myRend).material.SetColor("_OutlineColor", color);
                ((Renderer)__instance.myRend).material.SetColor("_AddColor", mainTarget ? color : Color.clear);
            }
        }
    }
}
