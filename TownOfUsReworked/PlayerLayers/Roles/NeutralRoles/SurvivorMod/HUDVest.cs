using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDVest
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Survivor))
                return;

            var role = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);

            if (role.VestButton == null)
                role.VestButton = Utils.InstantiateButton();

            role.VestButton.gameObject.SetActive(Utils.SetActive(role.Player, role.RoleType));

            role.VestButton.UpdateButton(role, "VEST", role.VestTimer(), CustomGameOptions.VestCd, TownOfUsReworked.VestSprite, AbilityTypes.Effect, role.Vesting, role.TimeRemaining,
                CustomGameOptions.VestDuration, true, !role.Vesting);
        }
    }
}