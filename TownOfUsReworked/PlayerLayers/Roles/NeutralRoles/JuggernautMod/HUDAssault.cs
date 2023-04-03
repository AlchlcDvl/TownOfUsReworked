using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JuggernautMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDAssault
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Juggernaut))
                return;

            var role = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);

            if (role.AssaultButton == null)
                role.AssaultButton = CustomButtons.InstantiateButton();

            role.AssaultButton.UpdateButton(role, "ASSAULT", role.KillTimer(), CustomGameOptions.JuggKillCooldown, AssetManager.Assault, AbilityTypes.Direct, "ActionSecondary");
        }
    }
}