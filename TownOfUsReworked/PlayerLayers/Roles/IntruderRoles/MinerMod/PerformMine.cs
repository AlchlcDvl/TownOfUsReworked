using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using Reactor.Networking.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MinerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformMine
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Miner))
                return true;

            var role = Role.GetRole<Miner>(PlayerControl.LocalPlayer);

            if (__instance == role.MineButton)
            {
                if (!role.CanPlace)
                    return false;

                if (role.MineTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Mine);
                var position = PlayerControl.LocalPlayer.transform.position;
                var id = Utils.GetAvailableId();
                writer.Write(id);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(position);
                writer.Write(position.z + 0.01f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.SpawnVent(id, role, position, position.z + 0.01f);
                role.LastMined = DateTime.UtcNow;
                return false;
            }

            return true;
        }
    }
}