using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDGaze
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Gorgon))
                return;

            var role = Role.GetRole<Gorgon>(PlayerControl.LocalPlayer);

            if (role.GazeButton == null)
                role.GazeButton = CustomButtons.InstantiateButton();

            var notGazed = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && !role.Gazed.Contains(x.PlayerId)).ToList();
            role.GazeButton.UpdateButton(role, "GAZE", role.GazeTimer(), CustomGameOptions.GazeCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", notGazed);

            if (!Role.SyndicateHasChaosDrive)
            {
                foreach (var id in role.Gazed)
                {
                    var player = Utils.PlayerById(id);

                    if (player.Data.IsDead)
                    {
                        if (!player.moveable)
                            player.moveable = true;

                        role.Gazed.Remove(id);
                        continue;
                    }

                    if (player.moveable)
                        player.moveable = false;
                }
            }
        }
    }
}