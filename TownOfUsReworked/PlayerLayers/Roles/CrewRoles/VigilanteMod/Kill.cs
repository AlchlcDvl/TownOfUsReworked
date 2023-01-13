using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Kill
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante);

            if (!flag)
                return true;

            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var flag2 = role.KillTimer() == 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled || role.ClosestPlayer == null)
                return false;

            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (!flag3)
                return false;
            
            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.Is(RoleEnum.Arsonist))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Arsonist))
                    ((Arsonist)pb).RpcSpreadDouse(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert() || role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastKilled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (role.Player.IsShielded())
                    {
                        var medic2 = role.Player.GetMedic().Player.PlayerId;
                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic2);
                        writer.Write(role.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        StopKill.BreakShield(medic2, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (!role.Player.IsProtected())
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
                        role.LastKilled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (!role.Player.IsProtected())
                    Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                    
                return false;
            }
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer1.Write(medic);
                writer1.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastKilled = DateTime.UtcNow;

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                return false;
            }
            else if (role.ClosestPlayer.IsVesting())
            {
                role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
                return false;
            }
            else if (role.ClosestPlayer.IsProtected())
            {
                role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                return false;
            }
            else if (role.Player.IsOtherRival(role.ClosestPlayer))
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }

            var flag4 = role.ClosestPlayer.Is(Faction.Intruder) || role.ClosestPlayer.Is(RoleAlignment.NeutralKill) || role.ClosestPlayer.Is(Faction.Syndicate) ||
                (role.ClosestPlayer.Is(RoleEnum.Jester) && CustomGameOptions.VigiKillsJester) || (role.ClosestPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) ||
                (role.ClosestPlayer.Is(RoleEnum.Cannibal) && CustomGameOptions.VigiKillsCannibal) || (role.ClosestPlayer.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB) ||
                role.ClosestPlayer.Is(RoleAlignment.NeutralNeo) || role.ClosestPlayer.Is(RoleAlignment.NeutralPros) || role.ClosestPlayer.IsRecruit() || role.ClosestPlayer.IsFramed() ||
                role.Player.IsTurnedTraitor() || role.Player.Is(ObjectifierEnum.Corrupted) || role.ClosestPlayer.Is(RoleEnum.Troll);
            
            if (flag4)
            {
                Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
                role.LastKilled = DateTime.UtcNow;
                role.KilledInno = false;
                return false;
            }
            else
            {
                if (CustomGameOptions.MisfireKillsInno)
                    Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
                
                if (CustomGameOptions.VigiKnowsInno)
                {
                    if (role.Player == PlayerControl.LocalPlayer && CustomGameOptions.VigiNotifOptions == VigiNotif.Flash && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                        Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    else if (CustomGameOptions.VigiNotifOptions == VigiNotif.Message)
                        role.InnoMessage = true;
                }
                
                if (CustomGameOptions.VigiKillAgain)
                {
                    role.LastKilled = DateTime.UtcNow;
                    role.KilledInno = false;
                }
                else
                    role.KilledInno = true;

                if (CustomGameOptions.VigiOptions == VigiOptions.Immediate)
                {
                    Utils.RpcMurderPlayer(role.Player, role.Player);
                    return false;
                }
                else if (CustomGameOptions.VigiOptions == VigiOptions.PreMeeting)
                    role.PreMeetingDie = true;
                else if (CustomGameOptions.VigiOptions == VigiOptions.PostMeeting)
                    role.PostMeetingDie = true;
            }

            return false;
        }
    }
}