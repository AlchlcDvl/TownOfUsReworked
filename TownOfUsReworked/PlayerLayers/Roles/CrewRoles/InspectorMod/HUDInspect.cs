using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDExamine
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Inspector))
                return;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (role.InspectButton == null)
                role.InspectButton = Utils.InstantiateButton();

            var notinspected = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Inspected.Contains(x.PlayerId)).ToList();
            role.InspectButton.UpdateButton(role, "INSPECT", role.InspectTimer(), CustomGameOptions.InspectCooldown, TownOfUsReworked.Placeholder, AbilityTypes.Direct,
                notinspected);
        }
    }
}