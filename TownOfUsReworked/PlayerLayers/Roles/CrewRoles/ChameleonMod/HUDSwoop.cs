using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDSwoop
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Chameleon))
                return;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            if (role.SwoopButton == null)
                role.SwoopButton = CustomButtons.InstantiateButton();

            role.SwoopButton.UpdateButton(role, "SWOOP", role.SwoopTimer(), CustomGameOptions.SwoopCooldown, AssetManager.Swoop, AbilityTypes.Effect, "ActionSecondary", null,
                role.ButtonUsable, role.ButtonUsable, role.IsSwooped, role.TimeRemaining, CustomGameOptions.SwoopDuration, true, role.UsesLeft);
        }
    }
}