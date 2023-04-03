using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDTrack
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Tracker))
                return;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (role.TrackButton == null)
                role.TrackButton = CustomButtons.InstantiateButton();

            var notTracked = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.TrackerArrows.ContainsKey(x.PlayerId)).ToList();
            role.TrackButton.UpdateButton(role, "TRACK", role.TrackerTimer(), CustomGameOptions.TrackCd, AssetManager.Track, AbilityTypes.Direct, "ActionSecondary", notTracked,
                role.ButtonUsable, role.ButtonUsable, false, 0, 1, true, role.UsesLeft);
        }
    }
}