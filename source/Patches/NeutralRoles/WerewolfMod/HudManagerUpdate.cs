using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.WerewolfMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite RampageSprite => TownOfUs.RampageSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf)) return;
            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.RampageKillCd);

            if (isDead)
            {
                role.RampageButton.gameObject.SetActive(false);
                return;
            }

            if (role.RampageButton == null)
            {
                role.RampageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RampageButton.graphic.enabled = true;
                role.RampageButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.RampageButton.gameObject.SetActive(false);
            }

            role.RampageButton.GetComponent<AspectPosition>().Update();
            role.RampageButton.graphic.sprite = RampageSprite;

            role.RampageButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.Rampaged)
            {
                role.RampageButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.RampageDuration);
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN);

                return;
            }
            else
            {
                role.RampageButton.SetCoolDown(role.RampageTimer(), CustomGameOptions.RampageCd);

                role.RampageButton.graphic.color = Palette.EnabledColor;
                role.RampageButton.graphic.material.SetFloat("_Desat", 0f);

                return;
            }
        }
    }
}
