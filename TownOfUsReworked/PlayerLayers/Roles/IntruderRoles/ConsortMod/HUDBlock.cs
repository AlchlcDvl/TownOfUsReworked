using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlock
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consort))
                return;
                
            var role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

            if (role.BlockButton == null)
                role.BlockButton = Utils.InstantiateButton();

            role.BlockButton.UpdateButton(role, "BLOCK", role.RoleblockTimer(), CustomGameOptions.ConsRoleblockCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", null,
                true, !role.Blocking, role.Blocking, role.TimeRemaining, CustomGameOptions.ConsRoleblockDuration);
        }
    }
}