using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDVest
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Survivor))
                return;

            var role = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);

            if (role.VestButton == null)
                role.VestButton = CustomButtons.InstantiateButton();

            role.VestButton.UpdateButton(role, "VEST", role.VestTimer(), CustomGameOptions.VestCd, AssetManager.Vest, AbilityTypes.Effect, "ActionSecondary", role.Vesting,
                role.TimeRemaining, CustomGameOptions.VestDuration, true, !role.Vesting);
        }
    }
}