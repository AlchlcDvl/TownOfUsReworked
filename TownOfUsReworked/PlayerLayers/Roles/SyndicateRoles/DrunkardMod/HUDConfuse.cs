using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDConfuse
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Drunkard))
                return;

            var role = Role.GetRole<Drunkard>(PlayerControl.LocalPlayer);

            if (role.ConfuseButton == null)
                role.ConfuseButton = CustomButtons.InstantiateButton();

            var flag = role.ConfusedPlayer == null && !role.HoldsDrive;
            role.ConfuseButton.UpdateButton(role, flag ? "SET CONFUSE" : "CONFUSE", role.DrunkTimer(), CustomGameOptions.ConfuseCooldown, AssetManager.Placeholder, AbilityTypes.Effect,
                "Secondary", role.Confused, role.TimeRemaining, CustomGameOptions.ConfuseDuration, true, !role.Confused);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.ConfusedPlayer != null && !role.HoldsDrive && !role.Confused)
                    role.ConfusedPlayer = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}