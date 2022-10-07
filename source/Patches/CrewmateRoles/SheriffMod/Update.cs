using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static string NameText(PlayerControl player, string str = "", bool meeting = false)
        {
            if (CamouflageUnCamouflage.IsCamoed)
            {
                if (meeting) return player.name + str;

                return "";
            }

            return player.name + str;
        }

        private static void UpdateMeeting(MeetingHud __instance, Sheriff sheriff)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!sheriff.Interrogated.Contains(player.PlayerId)) continue;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var roleType = Utils.GetRole(player);
                    switch (roleType)
                    {
                        default:
                            if (((player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Cannibal)) && !CustomGameOptions.NeutEvilRed) ||
                            ((player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Plaguebearer) || 
                            player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Werewolf)) && !CustomGameOptions.NeutKillingRed))
                            {
                                if (CustomGameOptions.SheriffAccuracy == 100 || sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy) {
                                    state.NameText.color = Patches.Colors.Glitch;
                                } else {
                                    state.NameText.color = Patches.Colors.Impostor;
                                }
                            }
                            else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                            {
                                foreach (var role in Role.GetRoles(RoleEnum.Traitor))
                                {
                                    var traitor = (Traitor)role;
                                    if (traitor.formerRole == RoleEnum.Vigilante || traitor.formerRole == RoleEnum.Veteran)
                                        state.NameText.color = Patches.Colors.Impostor;
                                    else state.NameText.color = Patches.Colors.Glitch;
                                }
                            }
                            else
                            {
                                if (CustomGameOptions.SheriffAccuracy == 100 || sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy) {
                                    state.NameText.color = Patches.Colors.Glitch;
                                } else {
                                    state.NameText.color = Patches.Colors.Impostor;
                                }
                            }
                            break;
                    }
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)

        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff)) return;
            var sheriff = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, sheriff);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!sheriff.Interrogated.Contains(player.PlayerId)) continue;
                var roleType = Utils.GetRole(player);
                player.nameText().transform.localPosition = new Vector3(0f, 2f, -0.5f);
                switch (roleType)
                {
                    default:
                        if (((player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Cannibal) ||
                            player.Is(RoleEnum.Phantom)) && !CustomGameOptions.NeutEvilRed) ||
                            ((player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Juggernaut) ||
                            player.Is(RoleEnum.Plaguebearer) || player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Werewolf)) && !CustomGameOptions.NeutKillingRed))
                        {
                            if (CustomGameOptions.SheriffAccuracy == 100 || sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy) {
                                player.nameText().color = Patches.Colors.Glitch;
                            } else {
                                player.nameText().color = Patches.Colors.Impostor;
                            }
                        }
                        else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                        {
                            foreach (var role in Role.GetRoles(RoleEnum.Traitor))
                            {
                                var traitor = (Traitor)role;
                                if (traitor.formerRole == RoleEnum.Sheriff || traitor.formerRole == RoleEnum.Vigilante ||
                                    traitor.formerRole == RoleEnum.Veteran) player.nameText().color = Patches.Colors.Impostor;
                                else player.nameText().color = Patches.Colors.Glitch;
                            }
                        }
                        else
                        {
                            if (CustomGameOptions.SheriffAccuracy == 100 || sheriff.randomSheriffAccuracy <= CustomGameOptions.SheriffAccuracy) {
                                player.nameText().color = Patches.Colors.Glitch;
                            } else {
                                player.nameText().color = Patches.Colors.Impostor;
                            }
                        }
                        break;
                }
            }
        }
    }
}