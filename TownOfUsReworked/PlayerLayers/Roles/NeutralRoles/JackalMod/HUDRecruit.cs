using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDRecruit
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

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jackal))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);
            var button = role.RecruitButton;

            if (button == null)
            {
                button = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                button.graphic.enabled = true;
            }

            button.GetComponent<AspectPosition>().Update();
            button.graphic.sprite = Placeholder;

            button.gameObject.SetActive(!MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead);

            var renderer = button.graphic;

            renderer.sprite = Placeholder;
            
            if (!button.isCoolingDown)
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