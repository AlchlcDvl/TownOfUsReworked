using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted))
                return false;

            var role = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted, role.ClosestPlayer, __instance) || __instance != role.KillButton)
                return false;

            if (role.KillTimer() != 0f && __instance == role.KillButton)
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer.Write(medic);
                writer.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastKilled = DateTime.UtcNow;

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
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

            Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
            role.LastKilled = DateTime.UtcNow;
            return false;
        }
    }
}