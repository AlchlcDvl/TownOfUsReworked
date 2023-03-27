using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDBlock
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Escort))
                return;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (role.BlockButton == null)
                role.BlockButton = CustomButtons.InstantiateButton();

            role.BlockButton.UpdateButton(role, "BLOCK", role.RoleblockTimer(), CustomGameOptions.EscRoleblockCooldown, AssetManager.EscortRoleblock, AbilityTypes.Direct, "ActionSecondary",
                null, true, !role.Blocking, role.Blocking, role.TimeRemaining, CustomGameOptions.EscRoleblockDuration);
        }
    }
}