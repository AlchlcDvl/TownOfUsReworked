using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDTransport
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (role.TransportButton == null)
                role.TransportButton = CustomButtons.InstantiateButton();

            var flag1 = role.TransportPlayer1 == null;
            var flag2 = role.TransportPlayer2 == null;
            role.TransportButton.UpdateButton(role, flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET" : "TRANSPORT"), role.TransportTimer(), CustomGameOptions.TransportCooldown,
                AssetManager.Transport, AbilityTypes.Effect, "ActionSecondary", null, role.ButtonUsable, role.ButtonUsable, false, 0, 1, true, role.UsesLeft);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.TransportPlayer2 != null)
                    role.TransportPlayer2 = null;
                else if (role.TransportPlayer1 != null)
                    role.TransportPlayer1 = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}