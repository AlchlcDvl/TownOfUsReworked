using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consort))
                return false;

            var role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

            if (__instance == role.BlockButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.RoleblockTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.SerialKiller), false, false, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.ConsRoleblock);
                    writer.Write(PlayerControl.LocalPlayer);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                    role.BlockTarget = role.ClosestPlayer;
                    role.Block();
                }

                if (interact[0] == true)
                    role.LastBlock = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer1.Write(medic);
                    writer1.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer1);
                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastKilled = DateTime.UtcNow;
                }
                else if (role.ClosestPlayer.IsVesting())
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
                else if (role.ClosestPlayer.IsProtected())
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (PlayerControl.LocalPlayer.IsOtherRival(role.ClosestPlayer))
                    role.LastKilled = DateTime.UtcNow;
                else
                {
                    role.LastKilled = DateTime.UtcNow;
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                }

                return false;
            }

            return false;
        }
    }
}