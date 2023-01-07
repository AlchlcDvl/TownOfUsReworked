using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDStab
    {
        public static Sprite LustSprite => TownOfUsReworked.Placeholder;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
                return;

            var role = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.LustKillCd);

            if (role.BloodlustButton == null)
            {
                role.BloodlustButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BloodlustButton.graphic.enabled = true;
                role.BloodlustButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.BloodlustButton.gameObject.SetActive(false);
            }

            if (isDead)
            {
                role.BloodlustButton.gameObject.SetActive(false);
                return;
            }

            role.BloodlustButton.GetComponent<AspectPosition>().Update();
            role.BloodlustButton.graphic.sprite = LustSprite;

            role.BloodlustButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.Lusted)
            {
                role.BloodlustButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.BloodlustDuration);
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN);

                return;
            }
            else
            {
                role.BloodlustButton.SetCoolDown(role.LustTimer(), CustomGameOptions.BloodlustCd);

                role.BloodlustButton.graphic.color = Palette.EnabledColor;
                role.BloodlustButton.graphic.material.SetFloat("_Desat", 0f);

                return;
            }
        }
    }
}
