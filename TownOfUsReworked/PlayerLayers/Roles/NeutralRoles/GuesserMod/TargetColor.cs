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
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Guesser))
                return;

            var role = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (MeetingHud.Instance)
                UpdateMeeting(MeetingHud.Instance, role);

            role.TargetPlayer.nameText().color = role.Color;
            role.TargetPlayer.nameText().text += " π";

            if (!role.TargetPlayer.Data.IsDead && !role.TargetPlayer.Data.Disconnected && role.RemainingGuesses > 0)
                return;

            if (role.TargetGuessed)
                return;
            
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
            writer.Write((byte)TurnRPC.GuessToAct);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            GuessToAct(PlayerControl.LocalPlayer);
        }

        public static void GuessToAct(PlayerControl player)
        {
            var guess = Role.GetRole<Guesser>(player);
            Role newRole  = new Jester(player);
            newRole.RoleHistory.Add(guess);
            newRole.RoleHistory.AddRange(guess.RoleHistory);
            
            if (newRole.Player == PlayerControl.LocalPlayer)
                newRole.RegenTask();
        }
    }
}