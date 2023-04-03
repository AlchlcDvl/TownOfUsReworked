using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformBlock
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Escort))
                return true;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (__instance == role.BlockButton)
            {
                if (role.RoleblockTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.EscRoleblock);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                    role.BlockTarget = role.ClosestPlayer;

                    foreach (var layer in PlayerLayer.GetLayers(role.BlockTarget))
                        layer.IsBlocked = !layer.RoleBlockImmune;

                    role.Block();
                }
                else if (interact[0])
                    role.LastBlock = DateTime.UtcNow;
                else if (interact[1])
                    role.LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}