using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GATargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, GuardianAngel role)
        {
            if (CustomGameOptions.GAKnowsTargetRole)
                return;

            foreach (var player in __instance.playerStates)
            {
                if (player.TargetPlayerId == role.TargetPlayer.PlayerId)
                {
                    player.NameText.color = role.Color;
                    player.NameText.text += " ★";
                }
            }
        }

        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.GuardianAngel))
                return;

            var role = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null)
                UpdateMeeting(MeetingHud.Instance, role);

            role.TargetPlayer.nameText().color = role.Color;
            role.TargetPlayer.nameText().text += " ★";

            if (role.TargetPlayer == null)
                return;

            if (role.TargetAlive)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
            writer.Write((byte)TurnRPC.GAToSurv);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Object.Destroy(role.UsesText);
            GAToSurv(PlayerControl.LocalPlayer);
        }

        public static void GAToSurv(PlayerControl player)
        {
            var ga = Role.GetRole<GuardianAngel>(player);
            var newRole = new Survivor(player);
            newRole.RoleHistory.Add(ga);
            newRole.RoleHistory.AddRange(ga.RoleHistory);
            
            if (newRole.Player == PlayerControl.LocalPlayer)
                newRole.RegenTask();
        }
    }
}