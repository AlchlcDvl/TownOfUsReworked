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
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(ObjectifierEnum.Corrupted))
                return;

            var role = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.KillButton.gameObject.SetActive(false);
            }

            role.KillButton.GetComponent<AspectPosition>().Update();
            role.KillButton.graphic.sprite = TownOfUsReworked.Placeholder;
            role.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.CorruptedKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton);
        }
    }
}