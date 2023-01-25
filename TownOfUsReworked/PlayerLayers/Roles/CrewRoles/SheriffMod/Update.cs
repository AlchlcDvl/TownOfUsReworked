using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Sheriff sheriff)
        {
            foreach (var player2 in sheriff.Interrogated)
            {
                var player = Utils.PlayerById(player2);

                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) 
                        continue;

                    if ((player.Is(RoleAlignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed) || (player.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NeutKillingRed))
                        state.NameText.color = Colors.Intruder;
                    else if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                    {
                        foreach (var role in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                        {
                            var traitor = (Traitor)role;

                            if (traitor.Player == player && traitor.Turned)
                                state.NameText.color = Colors.Intruder;
                        }
                    }
                    else if (player.Is(ObjectifierEnum.Traitor) && !CustomGameOptions.TraitorColourSwap)
                        state.NameText.color = Colors.Glitch;
                    else
                        state.NameText.color = Colors.Intruder;
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

            foreach (var player2 in sheriff.Interrogated)
            {
                var player = Utils.PlayerById(player2);

                player.nameText().transform.localPosition = new Vector3(0f, 2f, -0.5f);

                if ((player.Is(RoleAlignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed) || (player.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NeutKillingRed))
                    player.nameText().color = Colors.Intruder;
                else if (player.Is(ObjectifierEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                {
                    foreach (var role in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                    {
                        var traitor = (Traitor)role;

                        if (traitor.Player == player && traitor.Turned)
                            player.nameText().color = Colors.Intruder;
                    }
                }
                else if (player.Is(ObjectifierEnum.Traitor) && !CustomGameOptions.TraitorColourSwap)
                    player.nameText().color = Colors.Glitch;
                else
                    player.nameText().color = Colors.Intruder;
            }
        }
    }
}