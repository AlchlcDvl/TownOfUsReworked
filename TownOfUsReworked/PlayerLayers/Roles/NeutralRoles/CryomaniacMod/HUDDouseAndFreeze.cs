using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDouseAndFreeze
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Cryomaniac))
                return;

            var role = Role.GetRole<Cryomaniac>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
                role.FreezeButton = Utils.InstantiateButton();

            if (role.DouseButton == null)
                role.DouseButton = Utils.InstantiateButton();

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.DousedPlayers.Contains(player.PlayerId)).ToList();
            role.DouseButton.UpdateButton(role, "DOUSE", role.DouseTimer(), CustomGameOptions.CryoDouseCooldown, TownOfUsReworked.DouseSprite, AbilityTypes.Direct, notDoused);
            role.FreezeButton.UpdateButton(role, "FREEZE", 0, 1, TownOfUsReworked.CryoFreezeSprite, AbilityTypes.Effect, role.DousedAlive > 0 && !role.FreezeUsed);
        }
    }
}
