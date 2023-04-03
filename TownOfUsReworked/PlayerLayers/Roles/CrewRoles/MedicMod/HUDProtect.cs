using HarmonyLib;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDProtect
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Medic))
                return;

            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);

            if (role.ShieldButton == null)
                role.ShieldButton = CustomButtons.InstantiateButton();

            var notShielded = PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.ShieldedPlayer).ToList();
            role.ShieldButton.UpdateButton(role, "SHIELD", 0, 1, AssetManager.Shield, AbilityTypes.Direct, "ActionSecondary", notShielded, !role.UsedAbility);
        }
    }
}
