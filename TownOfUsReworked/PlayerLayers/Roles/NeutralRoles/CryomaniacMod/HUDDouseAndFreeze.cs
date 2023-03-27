using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDouseAndFreeze
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Cryomaniac))
                return;

            var role = Role.GetRole<Cryomaniac>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
                role.FreezeButton = CustomButtons.InstantiateButton();

            if (role.DouseButton == null)
                role.DouseButton = CustomButtons.InstantiateButton();

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.DousedPlayers.Contains(player.PlayerId)).ToList();
            role.DouseButton.UpdateButton(role, "DOUSE", role.DouseTimer(), CustomGameOptions.CryoDouseCooldown, AssetManager.Douse, AbilityTypes.Direct, "ActionSecondary", notDoused);
            role.FreezeButton.UpdateButton(role, "FREEZE", 0, 1, AssetManager.CryoFreeze, AbilityTypes.Effect, "Secondary", role.DousedAlive > 0 && !role.FreezeUsed);
        }
    }
}