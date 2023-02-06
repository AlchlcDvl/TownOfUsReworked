using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Abilities.ButtonBarryMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static Sprite Button => TownOfUsReworked.ButtonSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, AbilityEnum.ButtonBarry))
                return;

            var role = Ability.GetAbility<ButtonBarry>(PlayerControl.LocalPlayer);

            if (role.ButtonButton == null)
            {
                role.ButtonButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                role.ButtonButton.graphic.enabled = true;
                role.ButtonButton.graphic.sprite = Button;
            }

            role.ButtonButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.ButtonUsed && PlayerControl.LocalPlayer.RemainingEmergencies > 0);
            role.ButtonButton.SetCoolDown(role.StartTimer(), CustomGameOptions.ButtonCooldown);
            var renderer = role.ButtonButton.graphic;

            if (__instance.UseButton != null)
            {
                var position1 = __instance.UseButton.transform.position;
                role.ButtonButton.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y, position1.z);
            }
            else
            {
                var position1 = __instance.PetButton.transform.position;
                role.ButtonButton.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y, position1.z);
            }

            if (!role.ButtonUsed && PlayerControl.LocalPlayer.RemainingEmergencies > 0)
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