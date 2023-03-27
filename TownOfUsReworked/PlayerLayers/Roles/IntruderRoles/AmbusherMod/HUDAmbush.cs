using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.AmbusherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDAmbush
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Ambusher))
                return;

            var role = Role.GetRole<Ambusher>(PlayerControl.LocalPlayer);

            if (role.AmbushButton == null)
                role.AmbushButton = CustomButtons.InstantiateButton();

            role.AmbushButton.UpdateButton(role, "AMBUSH", role.AmbushTimer(), CustomGameOptions.AmbushCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", role.OnAmbush,
                role.TimeRemaining, CustomGameOptions.AmbushDuration, true, !role.OnAmbush);
        }
    }
}