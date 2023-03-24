using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMark
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Ghoul))
                return;

            var role = Role.GetRole<Ghoul>(PlayerControl.LocalPlayer);

            if (role.MarkButton == null)
                role.MarkButton = Utils.InstantiateButton();

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder) && x != role.MarkedPlayer).ToList();
            role.MarkButton.UpdateButton(role, "MARK", role.MarkTimer(), CustomGameOptions.GhoulMarkCd, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", notImp,
                role.MarkedPlayer == null, role.MarkedPlayer == null, false, 0, 1, false, 0, true);
        }
    }
}