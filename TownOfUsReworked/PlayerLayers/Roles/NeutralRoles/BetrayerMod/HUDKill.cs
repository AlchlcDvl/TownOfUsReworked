using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BetrayerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Betrayer))
                return;

            var role = Role.GetRole<Betrayer>(PlayerControl.LocalPlayer);

            if (role.Faction != Faction.Intruder && role.Faction != Faction.Syndicate)
                return;

            if (role.KillButton == null)
                role.KillButton = Utils.InstantiateButton();

            var notMates = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(role.Faction)).ToList();
            role.KillButton.UpdateButton(role, "KILL", role.KillTimer(), CustomGameOptions.BetrayerKillCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary",
                notMates);
        }
    }
}