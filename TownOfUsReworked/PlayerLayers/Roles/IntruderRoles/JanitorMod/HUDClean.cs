using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDClean
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Janitor))
                return;

            var role = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);

            if (role.CleanButton == null)
                role.CleanButton = Utils.InstantiateButton();

            role.CleanButton.UpdateButton(role, "CLEAN", role.CleanTimer(), CustomGameOptions.JanitorCleanCd, AssetManager.Clean, AbilityTypes.Dead, "Secondary");
        }
    }
}