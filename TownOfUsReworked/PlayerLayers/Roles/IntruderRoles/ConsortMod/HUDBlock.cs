using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDBlock
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consort))
                return;

            var role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

            if (role.BlockButton == null)
                role.BlockButton = CustomButtons.InstantiateButton();

            role.BlockButton.UpdateButton(role, "BLOCK", role.RoleblockTimer(), CustomGameOptions.ConsRoleblockCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", null,
                true, !role.Blocking, role.Blocking, role.TimeRemaining, CustomGameOptions.ConsRoleblockDuration);
        }
    }
}