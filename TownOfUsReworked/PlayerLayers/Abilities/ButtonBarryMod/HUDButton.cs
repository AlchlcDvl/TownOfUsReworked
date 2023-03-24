using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities.ButtonBarryMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDButton
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, AbilityEnum.ButtonBarry))
                return;

            var role = Ability.GetAbility<ButtonBarry>(PlayerControl.LocalPlayer);
            role.ButtonButton ??= Utils.InstantiateButton();
            role.ButtonButton.UpdateButton(role, AssetManager.Button, "BUTTON", AbilityTypes.Effect, !role.ButtonUsed && PlayerControl.LocalPlayer.RemainingEmergencies > 0,
                role.StartTimer(), CustomGameOptions.ButtonCooldown, "Quarternary", true, PlayerControl.LocalPlayer.RemainingEmergencies);
        }
    }
}