using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Hazel;
using TownOfUsReworked.Data;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformBlock
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consort))
                return true;

            var role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

            if (__instance == role.BlockButton)
            {
                if (role.RoleblockTimer() != 0f)
                    return false;

                if (role.BlockTarget == null)
                    role.BlockMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.Player).ToList());
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.ConsRoleblock);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.BlockTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;

                    foreach (var layer in PlayerLayer.GetLayers(role.BlockTarget))
                        layer.IsBlocked = !layer.RoleBlockImmune;

                    role.Block();
                }

                return false;
            }

            return true;
        }
    }
}