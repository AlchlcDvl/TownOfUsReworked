using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDShoot
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante))
                return;

            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (role.ShootButton == null)
                role.ShootButton = CustomButtons.InstantiateButton();

            role.ShootButton.UpdateButton(role, "SHOOT", role.KillTimer(), CustomGameOptions.VigiKillCd, AssetManager.Shoot, AbilityTypes.Direct, "ActionSecondary", true, role.UsesLeft,
                role.ButtonUsable, role.ButtonUsable && !role.KilledInno);
        }
    }
}