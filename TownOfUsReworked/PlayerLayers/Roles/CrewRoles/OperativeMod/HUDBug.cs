using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBug
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Operative))
                return;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (role.BugButton == null)
                role.BugButton = Utils.InstantiateButton();

            role.BugButton.UpdateButton(role, "BUG", role.BugTimer(), CustomGameOptions.BugCooldown, AssetManager.Bug, AbilityTypes.Effect, "ActionSecondary", true, role.UsesLeft,
                role.ButtonUsable);
        }
    }
}
