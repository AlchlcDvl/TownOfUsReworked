using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HUDFix
    {
        private static Sprite Fix => TownOfUsReworked.EngineerFix;
        
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                return;

            if (__instance.KillButton == null)
                return;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
            
            __instance.KillButton.graphic.sprite = Fix;
            __instance.KillButton.SetCoolDown(0f, 10f);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && __instance.UseButton.isActiveAndEnabled &&
                !MeetingHud.Instance);

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (!ShipStatus.Instance)
                return;

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var camouflager = Role.GetRoleValue(RoleEnum.Camouflager);
            var camo = (Camouflager)camouflager;
            var concealer = Role.GetRoleValue(RoleEnum.Concealer);
            var conc = (Concealer)concealer;
            var shapeshifter = Role.GetRoleValue(RoleEnum.Shapeshifter);
            var ss = (Shapeshifter)shapeshifter;

            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var active = specials.Any(s => s.IsActive) | camo.Camouflaged | conc.Concealed | ss.Shapeshifted;
            var renderer = __instance.KillButton.graphic;
            
            if (active & !dummyActive & !role.UsedThisRound & __instance.KillButton.enabled)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}