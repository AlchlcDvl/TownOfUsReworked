using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShoot
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante))
                return;
                
            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (role.ShootButton == null)
                role.ShootButton = Utils.InstantiateButton();

            role.ShootButton.UpdateButton(role, "SHOOT", role.KillTimer(), CustomGameOptions.VigiKillCd, TownOfUsReworked.ShootSprite, AbilityTypes.Direct, true, role.UsesLeft,
                role.ButtonUsable, role.ButtonUsable && !role.KilledInno);
        }
    }
}
