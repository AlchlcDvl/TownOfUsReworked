using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDInterrogate
    {
        public static Sprite Interrogate => TownOfUsReworked.SeerSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Sheriff))
                return;

            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (role.InterrogateButton == null)
            {
                role.InterrogateButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InterrogateButton.graphic.enabled = true;
                role.InterrogateButton.graphic.sprite = Interrogate;
                role.InterrogateButton.gameObject.SetActive(false);
            }

            role.InterrogateButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.InterrogateButton.SetCoolDown(role.InterrogateTimer(), CustomGameOptions.InterrogateCd);
            var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Interrogated.Contains(x.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.InterrogateButton, notInvestigated);
            role.PrimaryButton = role.InterrogateButton;
            var renderer = role.InterrogateButton.graphic;

            if (role.ClosestPlayer != null && !role.InterrogateButton.isCoolingDown)
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
