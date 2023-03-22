using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDSwoop
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Chameleon))
                return;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            if (role.SwoopButton == null)
                role.SwoopButton = Utils.InstantiateButton();

            role.SwoopButton.UpdateButton(role, "SWOOP", role.SwoopTimer(), CustomGameOptions.SwoopCooldown, AssetManager.Swoop, AbilityTypes.Effect, "ActionSecondary", role.IsSwooped,
                role.TimeRemaining, CustomGameOptions.SwoopDuration, true, !role.IsSwooped);
        }
    }
}