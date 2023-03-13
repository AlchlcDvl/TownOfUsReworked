using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities.ButtonBarryMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDButton
    {
        public static Sprite Button => TownOfUsReworked.ButtonSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, AbilityEnum.ButtonBarry))
                return;

            var role = Ability.GetAbility<ButtonBarry>(PlayerControl.LocalPlayer);

            if (role.ButtonButton == null)
            {
                role.ButtonButton = Utils.InstantiateButton();
                role.ButtonButton.graphic.enabled = true;
                role.ButtonButton.buttonLabelText.enabled = true;
            }

            role.ButtonButton.UpdateButton(role, Button, "BUTTON", AbilityTypes.Effect, !role.ButtonUsed && PlayerControl.LocalPlayer.RemainingEmergencies > 0, role.StartTimer(),
                CustomGameOptions.ButtonCooldown, true, PlayerControl.LocalPlayer.RemainingEmergencies);
        }
    }
}