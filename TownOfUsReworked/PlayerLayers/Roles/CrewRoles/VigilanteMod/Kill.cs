using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Kill
    {
        private static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante))
                return false;

            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante, role.ClosestPlayer, __instance) || __instance != role.ShootButton)
                return false;

            if (role.KillTimer() != 0f && __instance == role.ShootButton)
                return false;

            Utils.Spread(PlayerControl.LocalPlayer, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                Utils.AlertKill(PlayerControl.LocalPlayer, role.ClosestPlayer, true);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastKilled = DateTime.UtcNow;

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
            else if (PlayerControl.LocalPlayer.IsOtherRival(role.ClosestPlayer))
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }

            var flag4 = role.ClosestPlayer.Is(Faction.Intruder) || role.ClosestPlayer.Is(RoleAlignment.NeutralKill) || role.ClosestPlayer.Is(Faction.Syndicate) ||
                (role.ClosestPlayer.Is(RoleEnum.Jester) && CustomGameOptions.VigiKillsJester) || (role.ClosestPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) ||
                (role.ClosestPlayer.Is(RoleEnum.Cannibal) && CustomGameOptions.VigiKillsCannibal) || (role.ClosestPlayer.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB) ||
                role.ClosestPlayer.Is(RoleAlignment.NeutralNeo) || role.ClosestPlayer.Is(RoleAlignment.NeutralPros) || role.ClosestPlayer.IsRecruit() || role.ClosestPlayer.IsFramed() ||
                PlayerControl.LocalPlayer.IsTurnedTraitor() || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Corrupted) || role.ClosestPlayer.Is(RoleEnum.Troll) ||
                role.ClosestPlayer.IsTurnedTraitor();
            
            if (flag4)
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                role.LastKilled = DateTime.UtcNow;
                role.KilledInno = false;
                return false;
            }
            else
            {
                if (CustomGameOptions.MisfireKillsInno)
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                
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
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                else if (CustomGameOptions.VigiOptions == VigiOptions.PreMeeting)
                    role.PreMeetingDie = true;
                else if (CustomGameOptions.VigiOptions == VigiOptions.PostMeeting)
                    role.PostMeetingDie = true;
            }

            return false;
        }
    }
}