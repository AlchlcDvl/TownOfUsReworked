using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, Faction.Intruder))
                return;

            if (PlayerControl.LocalPlayer.IntruderSided())
                return;

            var role = Role.GetRole<IntruderRole>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
                role.KillButton = CustomButtons.InstantiateButton();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Ghoul))
            {
                role.KillButton.gameObject.SetActive(false);
                return;
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            role.KillButton.UpdateButton(role, "KILL", role.KillTimer(), CustomGameOptions.IntKillCooldown, AssetManager.IntruderKill, AbilityTypes.Direct, "ActionSecondary", notImp);
        }
    }
}