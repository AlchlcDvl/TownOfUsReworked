using HarmonyLib;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    public static class MeetingExiledEnd
    {
        public static void Postfix(ExileController __instance)
        {
            var exiled = __instance.exiled;

            if (exiled == null)
                return;

            var player = exiled.Object;
            var role = Role.GetRole(player);

            if (role == null)
                return;

            if (role.RoleType == RoleEnum.Jester)
            {
                var jest = (Jester)role;
                jest.VotedOut = true;
                jest.SetHaunted(MeetingHud.Instance);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.JesterWin);
                writer.Write(jest.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}