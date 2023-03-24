using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDCamo
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Camouflager))
                return;

            var role = Role.GetRole<Camouflager>(PlayerControl.LocalPlayer);

            if (role.CamouflageButton == null)
                role.CamouflageButton = Utils.InstantiateButton();

            role.CamouflageButton.UpdateButton(role, "CAMOUFLAGE", role.CamouflageTimer(), CustomGameOptions.CamouflagerCd, AssetManager.Camouflage, AbilityTypes.Effect, "Secondary",
                null, CamouflageUnCamouflage.IsCamoed, !CamouflageUnCamouflage.IsCamoed, CamouflageUnCamouflage.IsCamoed, role.TimeRemaining, CustomGameOptions.CamouflagerDuration);
        }
    }
}