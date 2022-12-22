using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;


namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WerewolfMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf);

            if (!flag)
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);

            if (role.Player.inVent)
                return false;

            if (role.MaulTimer() != 0)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (!flag3)
                return false;

            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                        SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastMauled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                }

                if (role.Player.IsProtected())
                    role.LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);

                Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
            }
            
            if (role.ClosestPlayer.IsInfected() | PlayerControl.LocalPlayer.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                        SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastMauled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (!role.Player.IsProtected())
                        Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                }
                else if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                        SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastMauled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                    return false;
                }
            }
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                    SendOption.Reliable, -1);
                writer.Write(medic);
                writer.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastMauled = DateTime.UtcNow;

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (role.ClosestPlayer.IsVesting())
                role.LastMauled.AddSeconds(CustomGameOptions.VestKCReset);
            else if (role.ClosestPlayer.IsProtected())
                role.LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);

            role.LastMauled = DateTime.UtcNow;

            Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Maul,
                    SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            role.Maul();
            return false;
        }
    }
}