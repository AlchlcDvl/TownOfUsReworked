using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, Faction.Syndicate))
                return;

            if (PlayerControl.LocalPlayer.SyndicateSided())
                return;

            var role = Role.GetRole<SyndicateRole>(PlayerControl.LocalPlayer);

            if (role.Type == RoleEnum.Banshee)
            {
                role.KillButton.gameObject.SetActive(false);
                return;
            }

            if (role.KillButton == null)
                role.KillButton = CustomButtons.InstantiateButton();

            var notSyndicate = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            role.KillButton.UpdateButton(role, "KILL", role.KillTimer(), CustomGameOptions.ChaosDriveKillCooldown, AssetManager.SyndicateKill, AbilityTypes.Direct, "ActionSecondary",
                notSyndicate, Role.SyndicateHasChaosDrive || PlayerControl.LocalPlayer.Is(RoleEnum.Anarchist) || PlayerControl.LocalPlayer.Is(RoleEnum.Sidekick));
        }
    }
}