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

            if (!__instance.enabled | role.ClosestPlayer == null)
                return false;

            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (!flag3) return false;

            var flag4 = (role.ClosestPlayer.Data.IsImpostor() | role.ClosestPlayer.Is(RoleAlignment.NeutralKill) |
                role.ClosestPlayer.Is(Faction.Syndicate) | (role.ClosestPlayer.Is(RoleEnum.Jester) && CustomGameOptions.VigiKillsJester) |
                (role.ClosestPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) | (role.ClosestPlayer.Is(RoleEnum.Cannibal)
                && CustomGameOptions.VigiKillsCannibal) | (role.ClosestPlayer.Is(RoleEnum.Taskmaster) && CustomGameOptions.VigiKillsTaskmaster) |
                (role.ClosestPlayer.Is(RoleEnum.Cryomaniac) && CustomGameOptions.VigiKillsTaskmaster) |
                (role.ClosestPlayer.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB));

            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                return false;
            }
            
            if (role.ClosestPlayer.IsInfected() | role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer1.Write(medic);
                    writer1.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer1);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastKilled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                }
                else if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer1.Write(medic);
                    writer1.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer1);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastKilled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                }
                else
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);

                return false;
            }
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
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

            if (!flag4)
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);

                if (CustomGameOptions.VigiOptions == VigiOptions.Immediate)
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

                else if (CustomGameOptions.VigiOptions == VigiOptions.Meeting)
                {
                    if (CustomGameOptions.VigiKnowsInno)
                    {
                        role.LastKilled.AddSeconds(1000);

                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            if (player == PlayerControl.LocalPlayer)
                                Coroutines.Start(Utils.FlashCoroutine(Colors.Vigilante));
                        }
                    }
                    else 
                        role.LastKilled = DateTime.UtcNow;
                    
                    MeetingDeath.MeetingDeathHappen(role.Player);
                }
            }
            else
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                role.LastKilled = DateTime.UtcNow;
            }

            return false;
        }
    }
}
