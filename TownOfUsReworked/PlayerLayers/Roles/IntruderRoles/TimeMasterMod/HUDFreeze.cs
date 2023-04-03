using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDFreeze
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.TimeMaster))
                return;

            var role = Role.GetRole<TimeMaster>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
                role.FreezeButton = CustomButtons.InstantiateButton();

            role.FreezeButton.UpdateButton(role, "TIME FREEZE", role.FreezeTimer(), CustomGameOptions.FreezeCooldown, AssetManager.TimeFreeze, AbilityTypes.Effect, "Secondary", role.Frozen,
                role.TimeRemaining, CustomGameOptions.FreezeDuration, true, !role.Frozen);
        }
    }
}