using HarmonyLib;
using TownOfUsReworked.Enums;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner) role;

                if (exe.TargetPlayer == null && PlayerControl.LocalPlayer == exe.Player)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
                    writer.Write((byte)TurnRPC.ExeToJest);
                    writer.Write(exe.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    TargetColor.ExeToJest(exe.Player);
                }
            }
        }
    }
}