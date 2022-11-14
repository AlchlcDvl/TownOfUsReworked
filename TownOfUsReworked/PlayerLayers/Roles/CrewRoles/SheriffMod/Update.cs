using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static string NameText(PlayerControl player, string str = "", bool meeting = false)
        {
            if (CamouflageUnCamouflage.IsCamoed)
            {
                if (meeting)
                    return player.name + str;

                return "";
            }

            return player.name + str;
        }

        private static void UpdateMeeting(MeetingHud __instance, Sheriff sheriff)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!sheriff.Interrogated.Contains(player.PlayerId))
                    continue;

                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) 
                        continue;

                    var roleType = Utils.GetRole(player);

                    switch (roleType)
                    {
                        default:
                            if (((player.Is(RoleEnum.Executioner) | player.Is(RoleEnum.Jester) | player.Is(RoleEnum.Cryomaniac) |
                                player.Is(RoleEnum.Cannibal)) && !CustomGameOptions.NeutEvilRed) | ((player.Is(RoleEnum.Arsonist) |
                                player.Is(RoleEnum.Glitch) | player.Is(RoleEnum.Juggernaut) | player.Is(RoleEnum.Plaguebearer) |
                                player.Is(RoleEnum.Pestilence) | player.Is(RoleEnum.SerialKiller) | player.Is(RoleEnum.Murderer)) &&
                                !CustomGameOptions.NeutKillingRed))
                            {
                                if (CustomGameOptions.SheriffAccuracy == 100 | sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy)
                                    state.NameText.color = Colors.Glitch;
                                else
                                    state.NameText.color = Colors.Intruder;
                            }
                            else if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                            {
                                foreach (var role in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                                {
                                    var traitor = (Traitor)role;
                                    state.NameText.color = Colors.Intruder;
                                }
                            }
                            else if (player.Is(ObjectifierEnum.Traitor) && !CustomGameOptions.TraitorColourSwap)
                            {
                                foreach (var role in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                                {
                                    var traitor = (Traitor)role;
                                    state.NameText.color = Colors.Glitch;
                                }
                            }
                            else
                            {
                                if (CustomGameOptions.SheriffAccuracy == 100 | sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy)
                                    state.NameText.color = Colors.Glitch;
                                else
                                    state.NameText.color = Colors.Intruder;
                            }

                            break;
                    }
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
                return;

            var sheriff = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null)
                UpdateMeeting(MeetingHud.Instance, sheriff);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!sheriff.Interrogated.Contains(player.PlayerId))
                    continue;

                player.nameText().transform.localPosition = new Vector3(0f, 2f, -0.5f);

                if (((player.Is(RoleEnum.Executioner) | player.Is(RoleEnum.Jester) | player.Is(RoleEnum.Cannibal) |
                    player.Is(RoleEnum.Cryomaniac)) && !CustomGameOptions.NeutEvilRed) | ((player.Is(RoleEnum.Murderer) |
                    player.Is(RoleEnum.Arsonist) | player.Is(RoleEnum.Glitch) | player.Is(RoleEnum.Juggernaut) |
                    player.Is(RoleEnum.Plaguebearer) | player.Is(RoleEnum.Pestilence) | player.Is(RoleEnum.SerialKiller)) &&
                    !CustomGameOptions.NeutKillingRed))
                {
                    if (CustomGameOptions.SheriffAccuracy == 100 | sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy)
                        player.nameText().color = Colors.Glitch;
                    else
                        player.nameText().color = Colors.Intruder;
                }
                        
                if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                {
                    foreach (var role in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                    {
                        var traitor = (Traitor)role;
                        player.nameText().color = Colors.Intruder;
                    }
                }
                else if (player.Is(ObjectifierEnum.Traitor) && !CustomGameOptions.TraitorColourSwap)
                {
                    foreach (var role in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                    {
                        var traitor = (Traitor)role;
                        player.nameText().color = Colors.Glitch;
                    }
                }
                else
                {
                    if (CustomGameOptions.SheriffAccuracy == 100 | sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy)
                        player.nameText().color = Colors.Glitch;
                    else
                        player.nameText().color = Colors.Intruder;
                }
            }
        }
    }
}