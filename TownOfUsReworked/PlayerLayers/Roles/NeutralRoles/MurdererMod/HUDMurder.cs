using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.MurdererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMurder
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Murderer))
                return;

            var role = Role.GetRole<Murderer>(PlayerControl.LocalPlayer);

            if (role.MurderButton == null)
                role.MurderButton = CustomButtons.InstantiateButton();

            role.MurderButton.UpdateButton(role, "MURDER", role.KillTimer(), CustomGameOptions.MurdKCD, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}