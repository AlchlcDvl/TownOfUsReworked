using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SampleSprite => TownOfUs.SampleSprite;
        public static Sprite MorphSprite => TownOfUs.MorphSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Morphling)) return;

            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (role.MorphButton == null)
            {
                role.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MorphButton.graphic.enabled = true;
                role.MorphButton.graphic.sprite = SampleSprite;
                role.MorphButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.MorphButton.gameObject.SetActive(false);

            }

            role.MorphButton.GetComponent<AspectPosition>().Update();

            if (role.MorphButton.graphic.sprite != SampleSprite && role.MorphButton.graphic.sprite != MorphSprite)
                role.MorphButton.graphic.sprite = SampleSprite;

            role.MorphButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            if (role.MorphButton.graphic.sprite == SampleSprite)
            {
                role.MorphButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.MorphButton);
            }
            else
            {
                if (role.Morphed)
                {
                    role.MorphButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }

                role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                role.MorphButton.graphic.color = Palette.EnabledColor;
                role.MorphButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
