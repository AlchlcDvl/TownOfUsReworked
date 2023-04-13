using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDConceal
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Concealer))
                return;

            var role = Role.GetRole<Concealer>(PlayerControl.LocalPlayer);

            if (role.ConcealButton == null)
                role.ConcealButton = CustomButtons.InstantiateButton();

            var flag = role.ConcealedPlayer == null && !role.HoldsDrive;
            role.ConcealButton.UpdateButton(role, flag ? "SET CONCEAL" : "CONCEAL", role.ConcealTimer(), CustomGameOptions.ConcealCooldown, AssetManager.Placeholder, AbilityTypes.Effect,
                "Secondary", null, true, !role.Concealed, role.Concealed, role.TimeRemaining, CustomGameOptions.ConcealDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.ConcealedPlayer != null && !role.HoldsDrive && !role.Concealed)
                    role.ConcealedPlayer = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}