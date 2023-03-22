using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, Faction.Syndicate))
                return;

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Allied) || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Traitor) || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Fanatic))
                return;

            var role = Role.GetRole<SyndicateRole>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
                role.KillButton = Utils.InstantiateButton();

            var notSyndicate = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            role.KillButton.UpdateButton(role, "KILL", role.KillTimer(), CustomGameOptions.ChaosDriveKillCooldown, AssetManager.SyndicateKill, AbilityTypes.Direct, "ActionSecondary",
                notSyndicate, Role.SyndicateHasChaosDrive || PlayerControl.LocalPlayer.Is(RoleEnum.Anarchist) || PlayerControl.LocalPlayer.Is(RoleEnum.Sidekick));
        }
    }
}