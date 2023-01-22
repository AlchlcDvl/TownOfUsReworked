using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist) || PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown)
                return false;

            if (__instance == role.IgniteButton && role.DousedAlive > 0)
            {
                if (role.ClosestPlayerIgnite == null)
                    return false;

                var distBetweenPlayers2 = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayerIgnite);
                var flag3 = distBetweenPlayers2 < GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

                if (!flag3)
                    return false;

                if (!role.DousedPlayers.Contains(role.ClosestPlayerIgnite.PlayerId))
                    return false;

                role.LastIgnited = DateTime.UtcNow;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Ignite);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                
                try
                {
                    SoundManager.Instance.PlaySound(TownOfUsReworked.IgniteSound, false, 1f);
                } catch {}

                role.Ignite();
                return false;
            }
            else if (__instance == role.DouseButton)
            {
                if (role.DouseTimer() != 0 || __instance != DestroyableSingleton<HudManager>.Instance.KillButton || role.ClosestPlayerDouse == null)
                    return false;

                var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayerDouse);
                var flag2 = distBetweenPlayers < GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

                if (!flag2)
                    return false;

                if (role.DousedPlayers.Contains(role.ClosestPlayerDouse.PlayerId))
                    return false;

                if (role.ClosestPlayerDouse.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayerDouse, role.Player);
                }

                if (role.ClosestPlayerDouse.IsOnAlert() || role.ClosestPlayerDouse.Is(RoleEnum.Pestilence))
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
                        SoundManager.Instance.PlaySound(TownOfUsReworked.DouseSound, false, 1f);
                    } catch {}

                    return false;
                }
                else if (role.Player.IsOtherRival(role.ClosestPlayerDouse))
                {
                    role.LastDoused = DateTime.UtcNow;
                    return false;
                }

                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer2.Write((byte)ActionsRPC.Douse);
                writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                writer2.Write(role.ClosestPlayerDouse.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                role.DousedPlayers.Add(role.ClosestPlayerDouse.PlayerId);
                role.LastDoused = DateTime.UtcNow;
                role.LastIgnited = DateTime.UtcNow;
                __instance.SetTarget(null);
                return false;
            }

            return false;
        }
    }
}
