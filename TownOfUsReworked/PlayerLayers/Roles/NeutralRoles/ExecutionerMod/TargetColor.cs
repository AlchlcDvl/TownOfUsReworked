using HarmonyLib;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class TargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Executioner role)
        {
            foreach (var player in __instance.playerStates)
            {
                if (player.TargetPlayerId == role.TargetPlayer.PlayerId)
                {
                    player.NameText.color = role.Color;
                    player.NameText.text += " §";
                }
            }
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Executioner))
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (MeetingHud.Instance)
                UpdateMeeting(MeetingHud.Instance, role);

            if (!CustomGameOptions.ExeKnowsTargetRole)
            {
                role.TargetPlayer.nameText().color = role.Color;
                role.TargetPlayer.nameText().text += " §";
            }

            if (!role.TargetPlayer.Data.IsDead && !role.TargetPlayer.Data.Disconnected)
                return;

            if (role.TargetVotedOut)
                return;
            
            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ExeToJest, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            ExeToJes(PlayerControl.LocalPlayer);
        }

        public static void ExeToJes(PlayerControl player)
        {
            var exe = Role.GetRole<Executioner>(player);
            Role newRole;

            if (CustomGameOptions.OnTargetDead == OnTargetDead.Jester)
                newRole = new Jester(player);
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac)
                newRole = new Amnesiac(player);
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Survivor)
                newRole = new Survivor(player);
            else
                newRole = new Crewmate(player);

            newRole.RoleHistory.Add(exe);
            newRole.RoleHistory.AddRange(exe.RoleHistory);
            
            if (newRole.Player == PlayerControl.LocalPlayer)
                newRole.RegenTask();
        }
    }
}