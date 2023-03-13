using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDTrack
    {
        public static Sprite Track => TownOfUsReworked.TrackSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Tracker))
                return;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (role.TrackButton == null)
                role.TrackButton = Utils.InstantiateButton();

            var notTracked = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.TrackerArrows.ContainsKey(x.PlayerId)).ToList();
            role.TrackButton.UpdateButton(role, "TRACK", role.TrackerTimer(), CustomGameOptions.TrackCd, TownOfUsReworked.TrackSprite, AbilityTypes.Direct,
                notTracked, role.ButtonUsable, role.ButtonUsable, false, 0, 1, true, role.UsesLeft);
        }
    }
}