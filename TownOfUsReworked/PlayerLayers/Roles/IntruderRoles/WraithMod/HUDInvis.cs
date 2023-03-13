using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.WraithMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDInvis
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Wraith))
                return;

            var role = Role.GetRole<Wraith>(PlayerControl.LocalPlayer);

            if (role.InvisButton == null)
                role.InvisButton = Utils.InstantiateButton();

            role.InvisButton.UpdateButton(role, "INVIS", role.InvisTimer(), CustomGameOptions.InvisCd, TownOfUsReworked.InvisSprite, AbilityTypes.Effect,
                null, true, !role.IsInvis, role.IsInvis, role.TimeRemaining, CustomGameOptions.InvisDuration);
        }
    }
}