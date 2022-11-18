using System.Linq;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FreezerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDFreeze
    {
        public static Sprite Freeze => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Freezer))
                return;

            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var role = Role.GetRole<Freezer>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
            {
                role.FreezeButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.FreezeButton.graphic.enabled = true;
                role.FreezeButton.graphic.sprite = Freeze;
            }

            role.FreezeButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.FreezeButton.transform.localPosition = __instance.KillButton.transform.localPosition;

            __instance.KillButton.graphic.color = new Color(0, 0, 0, 0);
            __instance.KillButton.gameObject.SetActive(false);

            var notImpostor = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.FreezeButton, float.NaN, notImpostor);
            
            if (role.ClosestPlayer != null)
            {
                role._freezeButton.graphic.color = Palette.EnabledColor;
                role._freezeButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role._freezeButton.graphic.color = Palette.DisabledClear;
                role._freezeButton.graphic.material.SetFloat("_Desat", 1.0f);
            }
        }
    }
}