using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.CrusaderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDCrusade
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Crusader))
                return;

            var role = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);

            if (role.CrusadeButton == null)
                role.CrusadeButton = Utils.InstantiateButton();

            role.CrusadeButton.UpdateButton(role, "CRUSADE", role.CrusadeTimer(), CustomGameOptions.CrusadeCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary",
                role.OnCrusade, role.TimeRemaining, CustomGameOptions.CrusadeDuration, true, !role.OnCrusade);
        }
    }
}