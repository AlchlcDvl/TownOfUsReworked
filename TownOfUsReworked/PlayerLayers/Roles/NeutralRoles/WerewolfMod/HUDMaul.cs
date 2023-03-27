using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WerewolfMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMaul
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Werewolf))
                return;

            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);

            if (role.MaulButton == null)
                role.MaulButton = CustomButtons.InstantiateButton();

            role.MaulButton.UpdateButton(role, "MAUL", role.MaulTimer(), CustomGameOptions.MaulCooldown, AssetManager.Maul, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}