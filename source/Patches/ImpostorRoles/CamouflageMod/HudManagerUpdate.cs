using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CamouflageMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Camouflage => TownOfUs.Camouflage;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Camouflager)) return;
            var role = Role.GetRole<Camouflager>(PlayerControl.LocalPlayer);
            if (role.CamouflageButton == null)
            {
                role.CamouflageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CamouflageButton.name = "CamouflageButton";
                role.CamouflageButton.graphic.enabled = true;
                role.CamouflageButton.graphic.sprite = Camouflage;
                role.CamouflageButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.CamouflageButton.gameObject.SetActive(false);
            }
            role.CamouflageButton.GetComponent<AspectPosition>().Update();
            role.CamouflageButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.Enabled)
            {
                role.CamouflageButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.CamouflagerDuration);
                return;
            }

            role.CamouflageButton.SetCoolDown(role.CamouflageTimer(), CustomGameOptions.CamouflagerCd);
            role.CamouflageButton.graphic.color = Palette.EnabledColor;
            role.CamouflageButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}