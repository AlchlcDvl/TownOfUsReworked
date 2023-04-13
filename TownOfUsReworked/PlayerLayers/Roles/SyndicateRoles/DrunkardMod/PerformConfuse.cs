using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Data;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformConfuse
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Drunkard))
                return true;

            var role = Role.GetRole<Drunkard>(PlayerControl.LocalPlayer);

            if (__instance == role.ConfuseButton)
            {
                if (role.DrunkTimer() != 0f)
                    return false;

                if (role.Confused)
                    return false;

                if (role.HoldsDrive)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Confuse);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ConfuseDuration;
                    role.Confuse();
                    Reverse.ConfuseAll();
                }
                else if (role.ConfusedPlayer == null)
                    role.ConfuseMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.ConfusedPlayer).ToList());
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Confuse);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ConfusedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ConfuseDuration;
                    role.Confuse();
                    Reverse.ConfuseSingle(role.ConfusedPlayer);
                }

                return false;
            }

            return true;
        }
    }
}