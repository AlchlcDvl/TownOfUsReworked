using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDRevive
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Altruist))
                return;

            var role = Role.GetRole<Altruist>(PlayerControl.LocalPlayer);

            if (role.ReviveButton == null)
                role.ReviveButton = Utils.InstantiateButton();

            role.ReviveButton.UpdateButton(role, "REVIVE", 0, 1, AssetManager.Revive, AbilityTypes.Dead, "ActionSecondary", !role.ReviveUsed);
        }
    }
}