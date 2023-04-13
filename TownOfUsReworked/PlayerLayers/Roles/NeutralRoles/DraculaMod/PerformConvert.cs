using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformConvert
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Dracula))
                return true;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (__instance == role.BiteButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.ConvertTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, false, true);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Convert);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    writer.Write((byte)SubFaction.Undead);
                    writer.Write(role.Converted.Count >= CustomGameOptions.AliveVampCount);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RoleGen.Convert(role.ClosestPlayer.PlayerId, role.Player.PlayerId, SubFaction.Undead, role.Converted.Count >= CustomGameOptions.AliveVampCount);
                }

                if (interact[0])
                    role.LastBitten = DateTime.UtcNow;
                else if (interact[1])
                    role.LastBitten.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastBitten.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }
    }
}