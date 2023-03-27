using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MinerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMine
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Miner))
                return;

            var role = Role.GetRole<Miner>(PlayerControl.LocalPlayer);

            if (role.MineButton == null)
                role.MineButton = CustomButtons.InstantiateButton();

            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, Utils.GetSize(), 0);
            hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
            role.CanPlace = hits.Count == 0 && PlayerControl.LocalPlayer.moveable && !SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1;
            role.MineButton.UpdateButton(role, "MINE", role.MineTimer(), CustomGameOptions.MineCd, AssetManager.Mine, AbilityTypes.Effect, null, true, role.CanPlace);
        }
    }
}