using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDInteract
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Troll))
                return;

            var role = Role.GetRole<Troll>(PlayerControl.LocalPlayer);

            if (role.InteractButton == null)
                role.InteractButton = Utils.InstantiateButton();

            role.InteractButton.UpdateButton(role, "INTERACT", role.InteractTimer(), CustomGameOptions.InteractCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}