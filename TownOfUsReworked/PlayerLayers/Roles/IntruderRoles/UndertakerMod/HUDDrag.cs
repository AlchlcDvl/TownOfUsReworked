using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDrag
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Undertaker))
                return;

            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (role.DragButton == null)
                role.DragButton = Utils.InstantiateButton();

            role.DragButton.UpdateButton(role, "DRAG", role.DragTimer(), CustomGameOptions.DragCd, AssetManager.Drag, AbilityTypes.Dead, "Secondary", role.CurrentlyDragging == null);

            if (role.DropButton == null)
                role.DropButton = Utils.InstantiateButton();

            role.DropButton.UpdateButton(role, "DROP", 0, 1, AssetManager.Drop, AbilityTypes.Dead, "Secondary", role.CurrentlyDragging != null);
        }
    }
}