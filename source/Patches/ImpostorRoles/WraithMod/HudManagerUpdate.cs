using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite InvisSprite => TownOfUs.InvisSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Wraith)) return;
            var role = Role.GetRole<Wraith>(PlayerControl.LocalPlayer);

            if (role.InvisButton == null)
            {
                role.InvisButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InvisButton.graphic.enabled = true;
                role.InvisButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.InvisButton.gameObject.SetActive(false);
            }
            role.InvisButton.GetComponent<AspectPosition>().Update();
            role.InvisButton.graphic.sprite = InvisSprite;
            role.InvisButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.IsInvis)
            {
                role.InvisButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.InvisDuration);
                return;
            }

            role.InvisButton.SetCoolDown(role.InvisTimer(), CustomGameOptions.InvisCd);


            role.InvisButton.graphic.color = Palette.EnabledColor;
            role.InvisButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}