using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDRewind
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.TimeLord))
                return;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);

            if (role.RewindButton == null)
                role.RewindButton = Utils.InstantiateButton();

            role.RewindButton.UpdateButton(role, "TIMER", role.TimeLordRewindTimer(), role.GetCooldown(), AssetManager.Rewind, AbilityTypes.Effect, "ActionSecondary", true, role.UsesLeft,
                role.ButtonUsable, role.ButtonUsable && !RecordRewind.rewinding);
        }
    }
}