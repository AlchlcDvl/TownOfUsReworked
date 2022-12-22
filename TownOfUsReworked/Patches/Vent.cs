using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerVentPatch
    {
        public static void Postfix(HudManager __instance)
        {
            if (__instance.ImpostorVentButton == null | __instance.ImpostorVentButton.gameObject == null |
                __instance.ImpostorVentButton.IsNullOrDestroyed())
                return;

            bool active = PlayerControl.LocalPlayer != null && VentPatches.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer._cachedData)
                && !MeetingHud.Instance && !LobbyBehaviour.Instance;

            if (active != __instance.ImpostorVentButton.gameObject.active)
                __instance.ImpostorVentButton.gameObject.SetActive(active);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        public static bool CanVent(PlayerControl player, GameData.PlayerInfo playerInfo)
        {
            var undertaker = Role.GetRole<Undertaker>(player);
            var tunneler = Ability.GetAbility<Tunneler>(player);
            
            if (player.inVent)
                return true;

            if (playerInfo.IsDead)
                return false;
            
            if (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone)
                return true;

            if (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Noone)
                return false;
            
            if (player.Is(Faction.Intruders) && CustomGameOptions.IntrudersVent)
                return true;
            
            if (player.Is(Faction.Syndicate) && CustomGameOptions.SyndicateVent)
                return true;
            
            if (player.Is(AbilityEnum.Tunneler) && tunneler.TasksDone)
                return true;

            if ((player.Is(RoleEnum.Morphling) && !CustomGameOptions.MorphlingVent) | (player.Is(RoleEnum.Wraith) && !CustomGameOptions.WraithVent) |
                (player.Is(RoleEnum.Grenadier) && !CustomGameOptions.GrenadierVent) | (player.Is(RoleEnum.Teleporter) && !CustomGameOptions.TeleVent) |
                (player.Is(RoleEnum.Poisoner) && !CustomGameOptions.PoisonerVent) | ((player.Is(RoleEnum.Undertaker) &&
                CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Never) | (player.Is(RoleEnum.Undertaker) &&
                undertaker.CurrentlyDragging != null && CustomGameOptions.UndertakerVentOptions != UndertakerOptions.Body)))
                return false;

            if (player.Is(RoleEnum.Engineer) | (player.Is(RoleEnum.Murderer) && CustomGameOptions.MurdVent) | (player.Is(RoleEnum.Glitch) &&
                CustomGameOptions.GlitchVent) | (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) | (player.Is(RoleEnum.Pestilence) &&
                CustomGameOptions.PestVent) | (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) | (player.Is(RoleEnum.Plaguebearer) &&
                CustomGameOptions.PBVent) | (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) | (player.Is(RoleEnum.Executioner) &&
                CustomGameOptions.ExeVent) | (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent) |
                (player.Is(RoleEnum.Dracula) && CustomGameOptions.DracVent) | (player.Is(RoleEnum.Vampire) && CustomGameOptions.VampVent) |
                (player.Is(RoleEnum.SerialKiller) && CustomGameOptions.SKVentOptions == SKVentOptions.Always) | (player.Is(RoleEnum.Survivor) &&
                CustomGameOptions.SurvVent) | (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent) | (player.Is(RoleEnum.Amnesiac) &&
                CustomGameOptions.AmneVent) | (player.Is(RoleEnum.Undertaker) && CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Always) |
                (player.Is(RoleEnum.Undertaker) && undertaker.CurrentlyDragging != null && CustomGameOptions.UndertakerVentOptions ==
                UndertakerOptions.Body) | (player.Is(RoleEnum.Undertaker) && undertaker.CurrentlyDragging == null &&
                CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Bodyless) | (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent))
                return true;

            if (player.Is(RoleEnum.SerialKiller) && CustomGameOptions.SKVentOptions == SKVentOptions.Bloodlust)
            {
                var role2 = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);

                if (role2.Lusted)
                    return true;
            }

            return false;
        }

        public static void Postfix(Vent __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse, ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;
            couldUse = CanVent(playerControl, playerInfo) && !playerControl.MustCleanVent(__instance.Id) && (!playerInfo.IsDead |
                playerControl.inVent) && (playerControl.CanMove | playerControl.inVent);
            var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

            if (ventitaltionSystem != null && ventitaltionSystem.PlayersCleaningVents != null)
            {
                foreach (var item in ventitaltionSystem.PlayersCleaningVents.Values)
                {
                    if (item == __instance.Id)
                        couldUse = false;
                }

            }
            canUse = couldUse;

            if (SubmergedCompatibility.isSubmerged())
            {
                if (SubmergedCompatibility.getInTransition())
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
                canUse = ((canUse ? 1 : 0) & ((double)num > (double)__instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
            }

            __result = num;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class JesterEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
            {
                if (CustomGameOptions.JestVentSwitch)
                    return true;
                else
                    return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class ExeEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent)
            {
                if (CustomGameOptions.ExeVentSwitch)
                    return true;
                else
                    return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class SurvEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent)
            {
                if (CustomGameOptions.SurvVentSwitch)
                    return true;
                else
                    return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class AmneEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent)
            {
                if (CustomGameOptions.AmneVentSwitch)
                    return true;
                else
                    return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class GAEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent)
            {
                if (CustomGameOptions.GAVentSwitch)
                    return true;
                else
                    return false;
            }

            return true;
        }
    }
}