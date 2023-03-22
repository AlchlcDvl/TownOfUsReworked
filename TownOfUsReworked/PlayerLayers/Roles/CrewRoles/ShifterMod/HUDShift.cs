using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShift
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shifter))
                return;

            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            if (role.ShiftButton == null)
                role.ShiftButton = Utils.InstantiateButton();

            role.ShiftButton.UpdateButton(role, "SHIFT", role.ShiftTimer(), CustomGameOptions.ShifterCd, AssetManager.Shift, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}
