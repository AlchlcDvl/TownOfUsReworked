using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformConvertButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jackal))
                return true;

            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null || role.RecruitTimer() != 0f || !__instance.enabled || role.HasRecruited)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer) > maxDistance)
                return false;

            var playerId = role.ClosestPlayer.PlayerId;

            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert() || role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastRecruited = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (!role.Player.IsProtected())
                    Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                    
                return false;
            }
            else if (role.Player.IsOtherRival(role.ClosestPlayer))
            {
                role.LastRecruited = DateTime.UtcNow;
                return false;
            }

            var target = role.ClosestPlayer;
            role.BackupRecruit = target;
            role.HasRecruited = true;
            var targetRole = Role.GetRole(target);
            targetRole.SubFaction = SubFaction.Cabal;
            targetRole.IsRecruit = true;
            role.RecruitButton.gameObject.SetActive(false);

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBackupRecruit, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            return false;
        }
    }
}
