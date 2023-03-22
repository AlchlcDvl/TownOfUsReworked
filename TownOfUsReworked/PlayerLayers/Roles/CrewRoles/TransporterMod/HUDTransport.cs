using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDTransport
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (role.TransportButton == null)
                role.TransportButton = Utils.InstantiateButton();

            role.TransportButton.UpdateButton(role, "TRANSPORT", role.TransportTimer(), CustomGameOptions.TransportCooldown, AssetManager.Transport, AbilityTypes.Effect, "ActionSecondary",
                null, role.ButtonUsable, role.ButtonUsable, false, 0, 1, true, role.UsesLeft);
            role.TransportListUpdate(__instance);
        }
    }
}