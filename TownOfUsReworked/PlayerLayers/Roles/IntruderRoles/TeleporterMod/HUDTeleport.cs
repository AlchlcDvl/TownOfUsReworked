using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TeleporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDTeleport
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Teleporter))
                return;

            var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);

            if (role.TeleportButton == null)
                role.TeleportButton = Utils.InstantiateButton();

            role.TeleportButton.UpdateButton(role, "TELEPORT", role.TeleportTimer(), CustomGameOptions.TeleportCd, AssetManager.Teleport, AbilityTypes.Effect, null,
                role.TeleportPoint != new Vector3(0, 0, 0));

            if (role.MarkButton == null)
                role.MarkButton = Utils.InstantiateButton();

            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, Utils.GetSize(), 0);
            hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
            role.CanMark = hits.Count == 0 && PlayerControl.LocalPlayer.moveable && !SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1 && role.TeleportPoint !=
                PlayerControl.LocalPlayer.transform.position;
            role.MarkButton.UpdateButton(role, "MARK", role.MarkTimer(), CustomGameOptions.MarkCooldown, AssetManager.Mark, AbilityTypes.Effect, null, true,
                role.CanMark);
        }
    }
}
