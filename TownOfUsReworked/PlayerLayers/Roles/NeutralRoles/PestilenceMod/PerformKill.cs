using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PestilenceMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Pestilence>(PlayerControl.LocalPlayer);

            if (role.Player.inVent || role.KillTimer() != 0 || role.ClosestPlayer == null)
                return false;

            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            
            if (!flag3)
                return false;
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
            else if (role.Player.IsOtherRival(role.ClosestPlayer))
            {
                role.LastKill = DateTime.UtcNow;
                return false;
            }
            
            role.LastKill = DateTime.UtcNow;
            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
            return false;
        }
    }
}