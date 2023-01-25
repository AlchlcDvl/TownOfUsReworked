using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    //Vent and kill shit
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ToggleHighlight))]
    class ToggleHighlightPatch
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] bool active, [HarmonyArgument(1)] RoleTeamTypes team)
        {
            var player = PlayerControl.LocalPlayer;
            bool isActive = player != null  && !MeetingHud.Instance && (player.Is(RoleAlignment.NeutralKill) || player.Is(RoleEnum.Thief) ||
                player.Is(Faction.Intruder) || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) ||
                player.Is(RoleEnum.Cannibal) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Dracula) || player.Is(RoleEnum.Dampyr) ||
                player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Tracker) ||
                player.Is(RoleEnum.Vigilante) || player.Is(Faction.Syndicate) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Escort) ||
                player.Is(RoleEnum.Troll));

            if (isActive)
            {
                var role = Role.GetRole(player); 
                var color = new Color32(255, 255, 255, 255);

                if (role != null)
                    color = role.Color;

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
                var color = new Color32(255, 255, 255, 255);

                if (role != null)
                    color = role.Color;
                    
                ((Renderer)__instance.myRend).material.SetColor("_OutlineColor", color);
                ((Renderer)__instance.myRend).material.SetColor("_AddColor", mainTarget ? color : Color.clear);
            }
        }
    }
}
