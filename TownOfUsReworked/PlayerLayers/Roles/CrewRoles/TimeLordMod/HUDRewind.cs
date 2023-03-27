using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDRewind
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.TimeLord))
                return;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);

            if (role.RewindButton == null)
                role.RewindButton = CustomButtons.InstantiateButton();

            role.RewindButton.UpdateButton(role, "REWIND", role.TimeLordRewindTimer(), TimeLord.GetCooldown(), AssetManager.Rewind, AbilityTypes.Effect, "ActionSecondary", true, role.UsesLeft,
                role.ButtonUsable, role.ButtonUsable && !RecordRewind.rewinding);
        }
    }
}