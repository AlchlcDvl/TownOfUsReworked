using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TeleporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDTeleport
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Teleporter))
                return;

            var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);

            if (role.TeleportButton == null)
                role.TeleportButton = CustomButtons.InstantiateButton();

            role.TeleportButton.UpdateButton(role, "TELEPORT", role.TeleportTimer(), CustomGameOptions.TeleportCd, AssetManager.Teleport, AbilityTypes.Effect, null,
                role.TeleportPoint != new Vector3(0, 0, 0));

            if (role.MarkButton == null)
                role.MarkButton = CustomButtons.InstantiateButton();

            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, Utils.GetSize(), 0);
            hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
            role.CanMark = hits.Count == 0 && PlayerControl.LocalPlayer.moveable && !SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1 && role.TeleportPoint !=
                PlayerControl.LocalPlayer.transform.position;
            role.MarkButton.UpdateButton(role, "MARK", role.MarkTimer(), CustomGameOptions.MarkCooldown, AssetManager.Mark, AbilityTypes.Effect, null, true, role.CanMark);
        }
    }
}