using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;

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

            var role = Role.GetRole<Troll>(PlayerControl.LocalPlayer);

            if (role.InteractButton == null)
            {
                role.InteractButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InteractButton.graphic.enabled = true;
                role.InteractButton.gameObject.SetActive(false);
            }

            role.InteractButton.GetComponent<AspectPosition>().Update();
            role.InteractButton.gameObject.SetActive(!MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead);
            Utils.SetTarget(ref role.ClosestPlayer, role.InteractButton);
            role.InteractButton.SetCoolDown(role.InteractTimer(), CustomGameOptions.InteractCooldown);
            var renderer = role.InteractButton.graphic;

            renderer.sprite = Placeholder;
            
            if (role.ClosestPlayer != null)
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