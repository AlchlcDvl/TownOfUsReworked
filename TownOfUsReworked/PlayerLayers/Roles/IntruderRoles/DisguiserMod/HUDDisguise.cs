using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;

using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;
        public static Sprite DisguiseSprite => TownOfUsReworked.DisguiseSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Disguiser))
                return;

            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);

            if (role.DisguiseButton == null)
            {
                role.DisguiseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DisguiseButton.graphic.enabled = true;
                role.DisguiseButton.graphic.sprite = MeasureSprite;
                role.DisguiseButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.DisguiseButton.gameObject.SetActive(false);

            }

            role.DisguiseButton.GetComponent<AspectPosition>().Update();

            if (role.DisguiseButton.graphic.sprite != MeasureSprite && role.DisguiseButton.graphic.sprite != DisguiseSprite)
                role.DisguiseButton.graphic.sprite = MeasureSprite;

            role.DisguiseButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.DisguiseButton.graphic.sprite == MeasureSprite)
            {
                role.DisguiseButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.DisguiseButton);
            }
            else
            {
                if (role.Disguised)
                {
                    role.DisguiseButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.DisguiseDuration);
                    return;
                }

                role.DisguiseButton.SetCoolDown(role.DisguiseTimer(), CustomGameOptions.DisguiseCooldown);

                Utils.SetTarget(ref role.ClosestPlayer, role.DisguiseButton, GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance]);
                role.DisguiseButton.graphic.color = Palette.EnabledColor;
                role.DisguiseButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
