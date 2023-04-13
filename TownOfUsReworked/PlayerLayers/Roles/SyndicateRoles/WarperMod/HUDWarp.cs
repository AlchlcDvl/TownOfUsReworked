using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDWarp
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Warper))
                return;

            var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

            if (role.WarpButton == null)
                role.WarpButton = CustomButtons.InstantiateButton();

            var flag1 = role.WarpPlayer1 == null && !role.HoldsDrive;
            var flag2 = role.WarpPlayer2 == null && !role.HoldsDrive;
            role.WarpButton.UpdateButton(role, flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET": "WARP"), role.WarpTimer(), CustomGameOptions.WarpCooldown, AssetManager.Placeholder,
                AbilityTypes.Effect, "ActionSecondary");

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.WarpPlayer2 != null && !role.HoldsDrive)
                    role.WarpPlayer2 = null;
                else if (role.WarpPlayer1 != null && !role.HoldsDrive)
                    role.WarpPlayer1 = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}