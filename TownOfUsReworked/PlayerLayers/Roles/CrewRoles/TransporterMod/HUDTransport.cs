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

            if (role.SetTransportButton1 == null)
                role.SetTransportButton1 = CustomButtons.InstantiateButton();

            if (role.SetTransportButton2 == null)
                role.SetTransportButton2 = CustomButtons.InstantiateButton();

            var flag1 = role.TransportPlayer1 == null;
            var flag2 = role.TransportPlayer2 == null;
            role.TransportButton.UpdateButton(role, "TRANSPORT", role.TransportTimer(), CustomGameOptions.TransportCooldown, AssetManager.Transport, AbilityTypes.Effect,
                "ActionSecondary", null, role.ButtonUsable && !(flag1 || flag2), role.ButtonUsable && !(flag1 || flag2), false, 0, 1, true, role.UsesLeft);
            role.SetTransportButton1.UpdateButton(role, "FIRST TARGET", role.TransportTimer(), CustomGameOptions.TransportCooldown, AssetManager.Transport, AbilityTypes.Effect,
                "ActionSecondary", null, role.ButtonUsable && flag1, role.ButtonUsable && flag1, false, 0, 1, true, role.UsesLeft);
            role.SetTransportButton2.UpdateButton(role, "SECOND TARGET", role.TransportTimer(), CustomGameOptions.TransportCooldown, AssetManager.Transport, AbilityTypes.Effect,
                "ActionSecondary", null, role.ButtonUsable && flag2, role.ButtonUsable && flag2, false, 0, 1, true, role.UsesLeft);

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