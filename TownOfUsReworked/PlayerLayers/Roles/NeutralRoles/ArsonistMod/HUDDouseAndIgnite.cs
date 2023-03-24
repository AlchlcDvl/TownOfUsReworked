using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDouseAndIgnite
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Arsonist))
                return;

            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            if (role.DouseButton == null)
                role.DouseButton = Utils.InstantiateButton();

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.DousedPlayers.Contains(player.PlayerId)).ToList();
            role.DouseButton.UpdateButton(role, "DOUSE", role.DouseTimer(), CustomGameOptions.DouseCd, AssetManager.Douse, AbilityTypes.Direct, "ActionSecondary", notDoused);

            if (role.IgniteButton == null)
                role.IgniteButton = Utils.InstantiateButton();

            role.IgniteButton.UpdateButton(role, "IGNITE", role.LastKiller && CustomGameOptions.ArsoLastKillerBoost ? 0 : role.IgniteTimer(), CustomGameOptions.IgniteCd,
                AssetManager.Ignite, AbilityTypes.Effect, "Secondary", role.DousedAlive > 0);
        }
    }
}
