using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

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
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Executioner))
                return;

            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (MeetingHud.Instance)
                UpdateMeeting(MeetingHud.Instance, role);

            role.TargetPlayer.nameText().color = role.Color;
            role.TargetPlayer.nameText().text += " §";

            if (!role.TargetPlayer.Data.IsDead && !role.TargetPlayer.Data.Disconnected)
                return;

            if (role.TargetVotedOut)
                return;
            
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
            writer.Write((byte)TurnRPC.ExeToJest);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            ExeToJest(PlayerControl.LocalPlayer);
        }

        public static void ExeToJest(PlayerControl player)
        {
            var exe = Role.GetRole<Executioner>(player);
            var newRole = new Jester(player);
            newRole.RoleHistory.Add(exe);
            newRole.RoleHistory.AddRange(exe.RoleHistory);
            
            if (newRole.Player == PlayerControl.LocalPlayer)
                newRole.RegenTask();
        }
    }
}