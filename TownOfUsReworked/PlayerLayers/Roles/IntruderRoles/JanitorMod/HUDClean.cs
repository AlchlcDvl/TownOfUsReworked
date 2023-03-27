using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDClean
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Janitor))
                return;

            var role = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);

            if (role.CleanButton == null)
                role.CleanButton = CustomButtons.InstantiateButton();

            role.CleanButton.UpdateButton(role, "CLEAN", role.CleanTimer(), CustomGameOptions.JanitorCleanCd, AssetManager.Clean, AbilityTypes.Dead, "Secondary");
        }
    }
}