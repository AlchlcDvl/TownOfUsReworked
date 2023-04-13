using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDShapeshift
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shapeshifter))
                return;

            var role = Role.GetRole<Shapeshifter>(PlayerControl.LocalPlayer);

            if (role.ShapeshiftButton == null)
                role.ShapeshiftButton = CustomButtons.InstantiateButton();

            var flag1 = role.ShapeshiftPlayer1 == null && !role.HoldsDrive;
            var flag2 = role.ShapeshiftPlayer2 == null && !role.HoldsDrive;
            role.ShapeshiftButton.UpdateButton(role, flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET": "SHAPESHIFT"), role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown,
                AssetManager.Shapeshift, AbilityTypes.Effect, "Secondary", role.Shapeshifted, role.TimeRemaining, CustomGameOptions.ShapeshiftDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.ShapeshiftPlayer2 != null && !role.HoldsDrive && !role.Shapeshifted)
                    role.ShapeshiftPlayer2 = null;
                else if (role.ShapeshiftPlayer1 != null && !role.HoldsDrive && !role.Shapeshifted)
                    role.ShapeshiftPlayer1 = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}