using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BetrayerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Betrayer))
                return;

            var role = Role.GetRole<Betrayer>(PlayerControl.LocalPlayer);

            if (role.Faction == Faction.Neutral)
                return;

            if (role.KillButton == null)
                role.KillButton = CustomButtons.InstantiateButton();

            var notMates = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(role.Faction)).ToList();
            role.KillButton.UpdateButton(role, "KILL", role.KillTimer(), CustomGameOptions.BetrayerKillCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary",
                notMates);
        }
    }
}