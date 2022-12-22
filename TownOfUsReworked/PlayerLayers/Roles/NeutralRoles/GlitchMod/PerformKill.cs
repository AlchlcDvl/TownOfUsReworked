using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;

using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite HackSprite => TownOfUsReworked.HackSprite;
        public static Sprite MimicSprite => TownOfUsReworked.MimicSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Glitch);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove | PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(role.Player, target);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (!flag3)
                return false;

            if (__instance == role.GlitchButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.GlitchButton.graphic.sprite == HackSprite)
                {
                    if (target == null)
                        return false;

                    role.HackTarget = target;
                    role.GlitchButton.SetTarget(target);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target);

                    if (role.HackTimer() < 5f)
                        role.LastMimic = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MimicCooldown);

                    try
                    {
                        SoundManager.Instance.PlaySound(TownOfUsReworked.HackSound, false, 1f);
                    } catch {}
                }
                else
                {
                    if (__instance.isCoolingDown)
                        return false;
                        
                    if (role.MimicTimer() != 0)
                        return false;

                    unchecked
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Morph,
                            SendOption.Reliable, -1);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.MimicTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    
                    role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    Utils.Morph(role.Player, role.MimicTarget);
                    
                    try
                    {
                        SoundManager.Instance.PlaySound(TownOfUsReworked.MorphSound, false, 1f);
                    } catch {}
                }
                
                if (role.ClosestPlayer.IsInfected() | PlayerControl.LocalPlayer.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
                }

                if (role.ClosestPlayer.IsOnAlert() | role.ClosestPlayer.Is(RoleEnum.Pestilence))
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
                            role.LastKill = DateTime.UtcNow;

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
                            role.LastKill = DateTime.UtcNow;

                        StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else
                        Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                        
                    return false;
                }

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;
                
                if (role.ClosestPlayer.IsInfected() | PlayerControl.LocalPlayer.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
                }

                if (role.ClosestPlayer.IsOnAlert() | role.ClosestPlayer.Is(RoleEnum.Pestilence))
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
                            role.LastKill = DateTime.UtcNow;

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
                            role.LastKill = DateTime.UtcNow;

                        StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else
                        Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                        
                    return false;
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
                        role.LastKill = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    return false;
                }
                else if (role.ClosestPlayer.IsVesting())
                {
                    role.LastKill.AddSeconds(CustomGameOptions.VestKCReset);

                    return false;
                }
                else if (role.ClosestPlayer.IsProtected())
                {
                    role.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset);

                    return false;
                }
            }

            return true;
        }
    }
}
