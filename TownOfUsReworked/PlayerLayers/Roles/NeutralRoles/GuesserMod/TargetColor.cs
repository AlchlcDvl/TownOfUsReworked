using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GuessTargetColor
    {
        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Guesser))
                return;

            var role = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

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
            newRole.PretendRoles = targetRole.InspectorResults;
            player.RegenTask();
        }
    }
}