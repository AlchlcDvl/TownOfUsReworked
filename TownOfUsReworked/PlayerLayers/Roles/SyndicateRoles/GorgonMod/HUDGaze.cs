using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDGaze
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Gorgon))
                return;

            var role = Role.GetRole<Gorgon>(PlayerControl.LocalPlayer);

            if (role.GazeButton == null)
                role.GazeButton = Utils.InstantiateButton();

            var notGazed = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && !role.Gazed.Contains(x.PlayerId)).ToList();
            role.GazeButton.UpdateButton(role, "GAZE", role.GazeTimer(), CustomGameOptions.GazeCooldown, TownOfUsReworked.Placeholder, AbilityTypes.Direct, notGazed);

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