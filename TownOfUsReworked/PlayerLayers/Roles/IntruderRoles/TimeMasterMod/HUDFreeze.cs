using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDFreeze
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.TimeMaster))
                return;

            var role = Role.GetRole<TimeMaster>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
                role.FreezeButton = Utils.InstantiateButton();

            role.FreezeButton.UpdateButton(role, "TIME FREEZE", role.FreezeTimer(), CustomGameOptions.FreezeCooldown, AssetManager.TimeFreeze, AbilityTypes.Effect, "Secondary", role.Frozen,
                role.TimeRemaining, CustomGameOptions.FreezeDuration, true, !role.Frozen);
        }
    }
}