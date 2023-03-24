using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PoisonerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDPoison
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Poisoner))
                return;

            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);

            if (role.PoisonButton == null)
                role.PoisonButton = Utils.InstantiateButton();

            var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList();
            role.PoisonButton.UpdateButton(role, role.Poisoned ? "POISONED" : "POISON", role.PoisonTimer(), CustomGameOptions.PoisonCd, role.Poisoned ? AssetManager.Poisoned :
                AssetManager.Poison, AbilityTypes.Direct, "Secondary", notSyn, true, !role.Poisoned, role.Poisoned && !Role.SyndicateHasChaosDrive, role.TimeRemaining,
                CustomGameOptions.PoisonDuration);
        }
    }
}
