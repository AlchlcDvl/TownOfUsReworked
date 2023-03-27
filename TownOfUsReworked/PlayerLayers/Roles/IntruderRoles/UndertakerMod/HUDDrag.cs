using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;

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
                role.DragButton = CustomButtons.InstantiateButton();

            role.DragButton.UpdateButton(role, "DRAG", role.DragTimer(), CustomGameOptions.DragCd, AssetManager.Drag, AbilityTypes.Dead, "Secondary", role.CurrentlyDragging == null);

            if (role.DropButton == null)
                role.DropButton = CustomButtons.InstantiateButton();

            role.DropButton.UpdateButton(role, "DROP", 0, 1, AssetManager.Drop, AbilityTypes.Effect, "Secondary", role.CurrentlyDragging != null && role.ClosestVent == null);

            if (role.HideButton == null)
                role.HideButton = CustomButtons.InstantiateButton();

            role.HideButton.UpdateButton(role, "HIDE BODY", 0, 1, AssetManager.Placeholder, AbilityTypes.Vent, "Secondary", role.CurrentlyDragging != null && role.ClosestVent != null);
        }
    }
}