using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.DetectiveMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDExamine
    {
        public static Sprite Examine => TownOfUsReworked.ExamineSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Detective))
                return;

            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);

            if (role.ExamineButton == null)
            {
                role.ExamineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ExamineButton.graphic.enabled = true;
                role.ExamineButton.graphic.sprite = Examine;
                role.ExamineButton.gameObject.SetActive(false);
            }

            role.ExamineButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.ExamineButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.ExamineCd);
            Utils.SetTarget(ref role.ClosestPlayer, role.ExamineButton);
            role.PrimaryButton = role.ExamineButton;
            var renderer = role.ExamineButton.graphic;
            
            if (!role.ExamineButton.isCoolingDown && role.ClosestPlayer != null)
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