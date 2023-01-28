using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMaul
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted))
                return;

            var role = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.graphic.sprite = TownOfUsReworked.Placeholder;
                role.KillButton.gameObject.SetActive(false);
            }

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.CorruptedKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton);
        }
    }
}