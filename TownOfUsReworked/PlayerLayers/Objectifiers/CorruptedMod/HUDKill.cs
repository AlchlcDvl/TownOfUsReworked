using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static Sprite CorruptedKill => TownOfUsReworked.CorruptedKill;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted))
                return;

            var role = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Utils.InstantiateButton();
                role.KillButton.graphic.enabled = true;
                role.KillButton.buttonLabelText.enabled = true;
            }

            role.KillButton.UpdateButton(role, role.KillTimer(), CustomGameOptions.CorruptedKillCooldown, "KILL", CorruptedKill, AbilityTypes.Direct);
        }
    }
}