using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlock
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Escort))
                return;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (role.BlockButton == null)
                role.BlockButton = Utils.InstantiateButton();

            role.BlockButton.UpdateButton(role, "BLOCK", role.RoleblockTimer(), CustomGameOptions.EscRoleblockCooldown, TownOfUsReworked.Placeholder, AbilityTypes.Direct, null,
                true, !role.Blocking, role.Blocking, role.TimeRemaining, CustomGameOptions.EscRoleblockDuration);
        }
    }
}