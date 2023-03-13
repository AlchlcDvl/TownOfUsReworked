using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JuggernautMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDAssault
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Juggernaut))
                return;

            var role = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);

            if (role.AssaultButton == null)
                role.AssaultButton = Utils.InstantiateButton();

            role.AssaultButton.UpdateButton(role, "ASSAULT", role.KillTimer(), CustomGameOptions.JuggKillCooldown, TownOfUsReworked.AssaultSprite, AbilityTypes.Direct);
        }
    }
}