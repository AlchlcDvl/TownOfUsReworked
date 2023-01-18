using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(role.Player, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (!flag3)
                return false;

            if (__instance == role.KillButton || __instance == role.HackButton)
            {
                if (!__instance.isActiveAndEnabled || (__instance == role.HackButton && role.HackTimer() != 0) || (__instance == role.KillButton && role.KillTimer() != 0))
                    return false;
                
                if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
                }

                if (role.ClosestPlayer.IsOnAlert() || role.ClosestPlayer.Is(RoleEnum.Pestilence) || (role.ClosestPlayer.Is(RoleEnum.SerialKiller) && __instance == role.HackButton))
                {
                    if (role.ClosestPlayer.IsShielded())
                    {
                        var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(role.ClosestPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            role.LastKill = DateTime.UtcNow;

                        StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                        if (!role.Player.IsProtected())
                            Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                    }
                    else if (role.Player.IsShielded())
                    {
                        var medic = role.Player.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(role.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            role.LastKill = DateTime.UtcNow;

                        StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (!role.Player.IsProtected())
                        Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                        
                    return false;
                }
                else if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastKill = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    return false;
                }

                if (__instance == role.KillButton)
                {
                    if (role.ClosestPlayer.IsVesting())
                    {
                        role.LastKill.AddSeconds(CustomGameOptions.VestKCReset);
                        return false;
                    }
                    else if (role.ClosestPlayer.IsProtected())
                    {
                        role.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset);
                        return false;
                    }
                    else
                    {
                        Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
                        role.LastKill = DateTime.UtcNow;
                        return false;
                    }
                }
                else if (__instance == role.HackButton)
                {
                    Utils.Block(role.Player, role.ClosestPlayer);
                    role.LastHack = DateTime.UtcNow;
                    return false;
                }
            }
            else if (__instance == role.MimicButton)
            {
                if (!__instance.isActiveAndEnabled || role.MimicTimer() != 0)
                    return false;

                role.MimicList = null;
                role.MimicButtonPress();
                return false;
            }

            return false;
        }
    }
}