using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using TMPro;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static Sprite CorruptedKill => TownOfUsReworked.CorruptedKill;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted))
                return;

            var role = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.buttonLabelText = Object.Instantiate(__instance.KillButton.buttonLabelText, __instance.KillButton.buttonLabelText.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.graphic.sprite = CorruptedKill;
                role.KillButton.buttonLabelText.text = "Kill";
            }

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.CorruptedKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton);

            if (!role.KillButton.isCoolingDown && role.ClosestPlayer != null)
            {
                role.KillButton.graphic.color = Palette.EnabledColor;
                role.KillButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.KillButton.graphic.color = Palette.DisabledClear;
                role.KillButton.graphic.material.SetFloat("_Desat", 1f);
            }
        }
    }
}