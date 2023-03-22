﻿using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformBlock
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consort))
                return true;

            var role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

            if (__instance == role.BlockButton)
            {
                if (!Utils.ButtonUsable(role.BlockButton))
                    return false;

                if (role.RoleblockTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.ConsRoleblock);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                    role.BlockTarget = role.ClosestPlayer;
                    var targetRole = Role.GetRole(role.BlockTarget);
                    targetRole.IsBlocked = !targetRole.RoleBlockImmune;
                    role.Block();
                }
                else if (interact[0] == true)
                    role.LastBlock = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}