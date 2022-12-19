using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDInteract
    {
        public static Sprite Placeholder = TownOfUsReworked.Placeholder;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Troll))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var interactButton = __instance.KillButton;
            var role = Role.GetRole<Troll>(PlayerControl.LocalPlayer);

            if (isDead)
                interactButton.gameObject.SetActive(false);
            else
                interactButton.gameObject.SetActive(!MeetingHud.Instance);

            var renderer = interactButton.graphic;

            renderer.sprite = Placeholder;
            
            if (!interactButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}