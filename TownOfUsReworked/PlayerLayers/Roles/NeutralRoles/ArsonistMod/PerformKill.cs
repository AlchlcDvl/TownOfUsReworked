using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist);

            if (!flag)
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            if (role.DouseTimer() != 0)
                return false;

            if (role.IgniteTimer() != 0)
                return false;

            if (!__instance.isActiveAndEnabled | __instance.isCoolingDown)
                return false;

            if (__instance == role.IgniteButton && role.DousedAlive > 0)
            {
                if (role.ClosestPlayerIgnite == null)
                    return false;

                var distBetweenPlayers2 = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayerIgnite);
                var flag3 = distBetweenPlayers2 < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

                if (!flag3)
                    return false;

                if (!role.DousedPlayers.Contains(role.ClosestPlayerIgnite.PlayerId))
                    return false;

                if (role.ClosestPlayerIgnite.IsInfected() | role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayerIgnite, role.Player);
                }
                
                if (role.ClosestPlayerIgnite.IsOnAlert() | role.ClosestPlayerIgnite.Is(RoleEnum.Pestilence))
                {
                    if (role.Player.IsShielded())
                    {
                        var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                            SendOption.Reliable, -1);
                        writer3.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                        writer3.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer3);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                        if (CustomGameOptions.ShieldBreaks)
                            role.LastDoused = DateTime.UtcNow;

                        StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                        return false;
                    }
                    else if (!role.Player.IsProtected())
                    {
                        Utils.RpcMurderPlayer(role.ClosestPlayerIgnite, PlayerControl.LocalPlayer);
                        return false;
                    }
                    
                    role.LastIgnited = DateTime.UtcNow;
                    return false;
                }

                role.LastIgnited = DateTime.UtcNow;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Ignite,
                    SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.Ignite();
                return false;
            }

            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;
            
            if (role.ClosestPlayerDouse == null)
                return false;

            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayerDouse);
            var flag2 = distBetweenPlayers < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (!flag2)
                return false;

            if (role.DousedPlayers.Contains(role.ClosestPlayerDouse.PlayerId))
                return false;

            if (role.ClosestPlayerDouse.IsInfected() | role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayerDouse, role.Player);
            }

            if (role.ClosestPlayerDouse.IsOnAlert() | role.ClosestPlayerDouse.Is(RoleEnum.Pestilence))
            {
                if (role.Player.IsShielded())
                {
                    var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                        SendOption.Reliable, -1);
                    writer3.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                    writer3.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer3);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastDoused = DateTime.UtcNow;

                    StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    return false;
                }
                else if (!role.Player.IsProtected())
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayerDouse, PlayerControl.LocalPlayer);
                    return false;
                }

                role.LastDoused = DateTime.UtcNow;

                try
                {
                    AudioClip douseSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Douse.raw");
                    SoundManager.Instance.PlaySound(douseSFX, false, 0.4f);
                } catch {}

                return false;
            }

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.Douse, SendOption.Reliable, -1);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(role.ClosestPlayerDouse.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);
            role.DousedPlayers.Add(role.ClosestPlayerDouse.PlayerId);
            role.LastDoused = DateTime.UtcNow;
            role.LastIgnited = DateTime.UtcNow;

            __instance.SetTarget(null);
            return false;
        }
    }
}
