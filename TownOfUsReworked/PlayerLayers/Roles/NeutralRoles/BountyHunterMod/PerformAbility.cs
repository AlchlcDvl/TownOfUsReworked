using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformKill
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.BountyHunter))
                return true;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);

            if (__instance == role.GuessButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.CheckTimer() != 0f)
                    return false;

                if (role.ClosestPlayer != role.TargetPlayer)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    role.UsesLeft--;
                }
                else
                {
                    role.TargetFound = true;
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }

                return false;
            }
            else if (__instance == role.HuntButton)
            {
                if (role.ClosestPlayer != role.TargetPlayer && !role.TargetKilled)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    Utils.RpcMurderPlayer(role.Player, role.Player);
                    role.LastChecked = DateTime.UtcNow;
                }
                else if (role.ClosestPlayer == role.TargetPlayer && !role.TargetKilled)
                {
                    var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                    if (interact[3])
                        Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.BountyHunterWin);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TargetKilled = true;
                    role.LastChecked = DateTime.UtcNow;
                    role.Kaboom();
                }
                else
                {
                    var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                    if (interact[0] || interact[3])
                        role.LastChecked = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastChecked.AddSeconds(CustomGameOptions.ProtectKCReset);
                    else if (interact[2])
                        role.LastChecked.AddSeconds(CustomGameOptions.VestKCReset);
                }

                return false;
            }

            return true;
        }
    }
}