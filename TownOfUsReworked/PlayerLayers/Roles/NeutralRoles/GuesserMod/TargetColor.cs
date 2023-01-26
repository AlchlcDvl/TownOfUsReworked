using HarmonyLib;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GuessTargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Guesser role)
        {
            foreach (var player in __instance.playerStates)
            {
                if (player.TargetPlayerId == role.TargetPlayer.PlayerId)
                {
                    player.NameText.color = role.Color;
                    player.NameText.text += " π";
                }
            }
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.IsDead ||
                !PlayerControl.LocalPlayer.Is(RoleEnum.Guesser))
                return;

            var role = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (MeetingHud.Instance)
                UpdateMeeting(MeetingHud.Instance, role);

            role.TargetPlayer.nameText().color = role.Color;
            role.TargetPlayer.nameText().text += " π";

            if ((!role.TargetPlayer.Data.IsDead && !role.TargetPlayer.Data.Disconnected && role.RemainingGuesses > 0) || role.TargetGuessed)
                return;
            
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
            writer.Write((byte)TurnRPC.GuessToAct);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            GuessToAct(PlayerControl.LocalPlayer);
        }

        public static void GuessToAct(PlayerControl player)
        {
            var exe = Role.GetRole<Guesser>(player);
            Role newRole;

            if (CustomGameOptions.OnTargetGone == OnTargetGone.Jester)
                newRole = new Jester(player);
            else if (CustomGameOptions.OnTargetGone == OnTargetGone.Amnesiac)
                newRole = new Amnesiac(player);
            else if (CustomGameOptions.OnTargetGone == OnTargetGone.Survivor)
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