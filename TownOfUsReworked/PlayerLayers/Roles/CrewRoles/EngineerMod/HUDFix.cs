using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDFix
    {
        public static Sprite Fix => TownOfUsReworked.EngineerFix;
        
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Engineer))
                return;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);

            if (role.FixButton == null)
            {
                role.FixButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FixButton.graphic.enabled = true;
                role.FixButton.graphic.sprite = Fix;
                role.FixButton.gameObject.SetActive(false);
            }
            
            role.FixButton.SetCoolDown(0f, 10f);
            role.FixButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.UsedThisRound);

            if (!ShipStatus.Instance)
                return;

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var camouflager = Role.GetRoleValue<Camouflager>(RoleEnum.Camouflager);
            var concealer = Role.GetRoleValue<Concealer>(RoleEnum.Concealer);
            var shapeshifter = Role.GetRoleValue<Shapeshifter>(RoleEnum.Shapeshifter);

            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var active = specials.Any(s => s.IsActive) || camouflager.Camouflaged || concealer.Concealed || shapeshifter.Shapeshifted;
            var renderer = role.FixButton.graphic;
            
            if (active && !dummyActive && !role.UsedThisRound)
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