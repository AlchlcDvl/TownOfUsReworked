using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerVentPatch
    {
        public static void Postfix(HudManager __instance)
        {
            if (__instance.ImpostorVentButton == null || __instance.ImpostorVentButton.gameObject == null || __instance.ImpostorVentButton.IsNullOrDestroyed())
                return;

            bool active = PlayerControl.LocalPlayer != null && Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) && !MeetingHud.Instance && !LobbyBehaviour.Instance;

            if (active != __instance.ImpostorVentButton.gameObject.active)
                __instance.ImpostorVentButton.gameObject.SetActive(active);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        public static void Postfix(Vent __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, [HarmonyArgument(1)] ref bool canUse,  [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;

            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal)
            {
                couldUse = Utils.CanVent(playerControl, playerInfo) && !playerControl.MustCleanVent(__instance.Id) && (!playerInfo.IsDead || playerControl.inVent) && (playerControl.CanMove
                    || playerControl.inVent);
            }
            else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek && playerControl.Data.IsImpostor())
            {
                couldUse = false;
            }
            else
            {
                couldUse = canUse;
            }

            var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

            if (ventitaltionSystem?.PlayersCleaningVents != null)
            {
                foreach (var item in ventitaltionSystem.PlayersCleaningVents.Values)
                {
                    if (item == __instance.Id)
                        couldUse = false;
                }
            }

            canUse = couldUse;

            if (SubmergedCompatibility.IsSubmerged())
            {
                if (SubmergedCompatibility.GetInTransition())
                {
                    __result = float.MaxValue;
                    return;
                }

                switch (__instance.Id)
                {
                    case 9:  //Engine Room Exit Only Vent
                        if (PlayerControl.LocalPlayer.inVent)
                            break;

                        __result = float.MaxValue;
                        return;

                    case 14: // Lower Central
                        __result = float.MaxValue;

                        if (canUse)
                        {
                            Vector3 center = playerControl.Collider.bounds.center;
                            Vector3 position = __instance.transform.position;
                            __result = Vector2.Distance(center, position);
                            canUse &= __result <= __instance.UsableDistance;
                        }

                        return;
                }
            }

            if (canUse)
            {
                Vector3 center = playerControl.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance((Vector2)center, (Vector2)position);
                canUse = ((canUse ? 1 : 0) & ((double)num > (double)__instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center,
                    (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
            }

            __result = num;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class EnterVentPatch
    {
        public static bool Prefix()
        {
            var player = PlayerControl.LocalPlayer;

            if (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
                return CustomGameOptions.JestVentSwitch;
            else if (player.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent)
                return CustomGameOptions.ExeVentSwitch;
            else if (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent)
                return CustomGameOptions.SurvVentSwitch;
            else if (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent)
                return CustomGameOptions.AmneVentSwitch;
            else if (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent)
                return CustomGameOptions.GAVentSwitch;
            else if (player.Is(RoleEnum.Guesser) && CustomGameOptions.GuessVent)
                return CustomGameOptions.GuessVentSwitch;
            else if (player.Is(RoleEnum.Troll) && CustomGameOptions.TrollVent)
                return CustomGameOptions.TrollVentSwitch;
            else if (player.Is(RoleEnum.Actor) && CustomGameOptions.ActorVent)
                return CustomGameOptions.ActVentSwitch;
            else if (player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Banshee) || player.Is(RoleEnum.Ghoul))
                return false;
            else
                return true;
        }
    }
}