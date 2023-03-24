using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BeamerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDBeam
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Beamer))
                return;

            var role = Role.GetRole<Beamer>(PlayerControl.LocalPlayer);

            if (role.BeamButton == null)
                role.BeamButton = Utils.InstantiateButton();

            role.BeamButton.UpdateButton(role, "BEAM", role.BeamTimer(), CustomGameOptions.BeamCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary");
            role.BeamListUpdate(__instance);
        }
    }
}