using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.NeutralRoles.PlaguebearerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            // if (!__instance.isActiveAndEnabled) return false;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
            if (role.InfectTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            if (role.InfectedPlayers.Contains(role.ClosestPlayer.PlayerId)) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (!flag3) return false;
            if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.Player.IsShielded())
                {
                    var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer3.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                    writer3.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer3);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                    if (CustomGameOptions.ShieldBreaks)
                        role.LastInfected = DateTime.UtcNow;
                    StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    return false;
                }
                else if (!role.Player.IsProtected())
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    return false;
                }
                role.LastInfected = DateTime.UtcNow;
                return false;
            }

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.Infect, SendOption.Reliable, -1);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(role.ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);
            role.InfectedPlayers.Add(role.ClosestPlayer.PlayerId);
            role.LastInfected = DateTime.UtcNow;

            __instance.SetTarget(null);
            return false;
        }
    }
}