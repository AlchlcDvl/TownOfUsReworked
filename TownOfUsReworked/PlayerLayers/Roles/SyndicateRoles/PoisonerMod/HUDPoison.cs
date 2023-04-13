using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using UnityEngine;

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
                role.PoisonButton = CustomButtons.InstantiateButton();

            var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList();
            var flag = role.PoisonedPlayer == null && role.HoldsDrive;
            role.PoisonButton.UpdateButton(role, flag ? "SET POISON" : "POISON", role.PoisonTimer(), CustomGameOptions.PoisonCd, AssetManager.Poison, role.HoldsDrive ? AbilityTypes.Effect
                : AbilityTypes.Direct, "Secondary", notSyn, true, !role.Poisoned, role.Poisoned, role.TimeRemaining, CustomGameOptions.PoisonDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.PoisonedPlayer != null && role.HoldsDrive && !role.Poisoned)
                    role.PoisonedPlayer = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}
