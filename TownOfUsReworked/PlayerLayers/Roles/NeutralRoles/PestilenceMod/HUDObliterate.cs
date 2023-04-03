using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PestilenceMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDObliterate
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Pestilence))
                return;

            var role = Role.GetRole<Pestilence>(PlayerControl.LocalPlayer);

            if (role.ObliterateButton == null)
                role.ObliterateButton = CustomButtons.InstantiateButton();

            role.ObliterateButton.UpdateButton(role, "OBLITERATE", role.KillTimer(), CustomGameOptions.PestKillCd, AssetManager.Obliterate, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}