using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDClean
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Janitor))
                return;

            var role = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);

            if (role.CleanButton == null)
                role.CleanButton = CustomButtons.InstantiateButton();

            if (role.DragButton == null)
                role.DragButton = CustomButtons.InstantiateButton();

            if (role.DropButton == null)
                role.DropButton = CustomButtons.InstantiateButton();

            role.DragButton.UpdateButton(role, "DRAG", role.DragTimer(), CustomGameOptions.DragCd, AssetManager.Drag, AbilityTypes.Dead, "Tertiary", role.CurrentlyDragging == null);
            role.DropButton.UpdateButton(role, "DROP", 0, 1, AssetManager.Drop, AbilityTypes.Effect, "Tertiary", role.CurrentlyDragging != null);
            role.CleanButton.UpdateButton(role, "CLEAN", role.CleanTimer(), CustomGameOptions.JanitorCleanCd, AssetManager.Clean, AbilityTypes.Dead, "Secondary");
        }
    }
}