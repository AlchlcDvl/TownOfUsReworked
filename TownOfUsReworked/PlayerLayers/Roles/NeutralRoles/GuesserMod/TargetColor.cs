using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using Random = UnityEngine.Random;
using System.Linq;

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
            
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
            writer.Write((byte)TurnRPC.GuessToAct);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            GuessToAct(PlayerControl.LocalPlayer);
        }

        public static void GuessToAct(PlayerControl player)
        {
            var guess = Role.GetRole<Guesser>(player);
            var targetRole = Role.GetRole(guess.TargetPlayer);
            var newRole  = new Actor(player);
            newRole.RoleHistory.Add(guess);
            newRole.RoleHistory.AddRange(guess.RoleHistory);
            newRole.PretendRoles.Add(targetRole);
            var i = 0;

            while (i < 4)
            {
                var random = Random.RandomRangeInt(0, Role.AllRoles.Count());
                var role2 = Role.AllRoles.ToList()[random];

                if (role2.RoleType != targetRole.RoleType)
                {
                    newRole.PretendRoles.Add(role2);
                    i++;
                }

                newRole.PretendRoles.Shuffle();
            }
            
            player.RegenTask();
        }
    }
}