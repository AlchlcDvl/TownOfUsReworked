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

            bool active = PlayerControl.LocalPlayer != null && VentPatches.CanVent(PlayerControl.LocalPlayer)
                && !MeetingHud.Instance && !LobbyBehaviour.Instance;

            if (active != __instance.ImpostorVentButton.gameObject.active)
                __instance.ImpostorVentButton.gameObject.SetActive(active);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        public static bool CanVent(PlayerControl player)
        {
            bool mainflag = false;

            if (player.Data.IsDead || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Noone || player == null || player.Data == null || player.Data.Disconnected ||
                LobbyBehaviour.Instance || MeetingHud.Instance)
                mainflag = false;
            else if (player.inVent || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone)
                mainflag = true;
            else if (((player.IsRecruit() || (player.Is(RoleEnum.Recruit)) && CustomGameOptions.RecruitVent)) || (player.Is(RoleEnum.Jackal) && CustomGameOptions.JackalVent))
                mainflag = true;
            else if (player.Is(Faction.Syndicate))
                mainflag = CustomGameOptions.SyndicateVent;
            else if (player.Is(Faction.Intruder))
            {
                if (CustomGameOptions.IntrudersVent)
                {
                    var flag = (player.Is(RoleEnum.Morphling) && !CustomGameOptions.MorphlingVent) || (player.Is(RoleEnum.Wraith) && !CustomGameOptions.WraithVent) ||
                        (player.Is(RoleEnum.Grenadier) && !CustomGameOptions.GrenadierVent) || (player.Is(RoleEnum.Teleporter) && !CustomGameOptions.TeleVent) ||
                        (player.Is(RoleEnum.Poisoner) && !CustomGameOptions.PoisonerVent);

                    if (flag)
                        mainflag = !flag;
                    else if (player.Is(RoleEnum.Undertaker))
                    {
                        var undertaker = Role.GetRole<Undertaker>(player);
                
                        mainflag = CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Always || undertaker.CurrentlyDragging != null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Body || undertaker.CurrentlyDragging == null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Bodyless;
                    }
                }
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Crew))
            {            
                if (player.Is(AbilityEnum.Tunneler) && !player.Is(RoleEnum.Engineer))
                {
                    var tunneler = Ability.GetAbility<Tunneler>(player);
                    mainflag = tunneler.TasksDone;
                }
                else if (player.Is(RoleEnum.Engineer))
                    mainflag = true;
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Neutral))
            {
                var flag = (player.Is(RoleEnum.Murderer) && CustomGameOptions.MurdVent) || (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) ||
                    (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) || (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) ||
                    (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) || (player.Is(RoleEnum.Plaguebearer) && CustomGameOptions.PBVent) ||
                    (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) || (player.Is(RoleEnum.Executioner) &&  CustomGameOptions.ExeVent) ||
                    (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent) || (player.Is(RoleEnum.Dracula) && CustomGameOptions.DracVent) ||
                    (player.Is(RoleEnum.Vampire) && CustomGameOptions.VampVent) || (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent) ||
                    (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent) || (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent) ||
                    (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent) || (player.Is(RoleEnum.Dampyr) && CustomGameOptions.DampVent);

                if (flag)
                    mainflag = flag;
                else if (player.Is(RoleEnum.SerialKiller))
                {
                    var role2 = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);

                    if (CustomGameOptions.SKVentOptions == SKVentOptions.Always || (role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.Bloodlust) ||
                        (!role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.NoLust))
                        mainflag = true;
                    else
                        mainflag = false;
                }
                else
                    mainflag = false;
            }
            else
                mainflag = false;

            return mainflag;
        }

        public static void Postfix(Vent __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse, ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;
            couldUse = CanVent(playerControl) && !playerControl.MustCleanVent(__instance.Id) && (!playerInfo.IsDead |
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
    public static class EnterVentPatch
    {
        public static bool Prefix(Vent __instance)
        {
            var player = PlayerControl.LocalPlayer;

            if (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
            {
                if (CustomGameOptions.JestVentSwitch)
                    return true;
                else
                    return false;
            }
            else if (player.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent)
            {
                if (CustomGameOptions.ExeVentSwitch)
                    return true;
                else
                    return false;
            }
            else if (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent)
            {
                if (CustomGameOptions.SurvVentSwitch)
                    return true;
                else
                    return false;
            }
            else if (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent)
            {
                if (CustomGameOptions.AmneVentSwitch)
                    return true;
                else
                    return false;
            }
            else if (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent)
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