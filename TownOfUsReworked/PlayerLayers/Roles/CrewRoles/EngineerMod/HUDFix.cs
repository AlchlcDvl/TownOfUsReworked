using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
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

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(role.UsesText.transform.localPosition.x + 0.26f, role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }

            if (role.UsesText != null)
                role.UsesText.text = $"{role.UsesLeft}";
            
            role.FixButton.SetCoolDown(0f, 10f);
            role.FixButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);

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
            
            if (active && !dummyActive && role.ButtonUsable)
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