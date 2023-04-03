using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDBug
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Operative))
                return;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (role.BugButton == null)
                role.BugButton = CustomButtons.InstantiateButton();

            role.BugButton.UpdateButton(role, "BUG", role.BugTimer(), CustomGameOptions.BugCooldown, AssetManager.Bug, AbilityTypes.Effect, "ActionSecondary", true, role.UsesLeft,
                role.ButtonUsable);
        }
    }
}