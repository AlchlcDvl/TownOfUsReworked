using HarmonyLib;
using TownOfUsReworked.Classes;
using Hazel;
using System;
using TownOfUsReworked.Data;
using Reactor.Utilities;
using System.Linq;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformWarp
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Warper))
                return true;

            var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

            if (__instance == role.WarpButton)
            {
                if (role.WarpTimer() != 0f)
                    return false;

                if (role.HoldsDrive)
                {
                    Utils.Warp();
                    role.LastWarped = DateTime.UtcNow;
                }
                else
                {
                    var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == role.WarpPlayer1 || x == role.WarpPlayer2 || (x == role.Player && !CustomGameOptions.WarpSelf)
                        || (x.Data.IsDead && Utils.BodyById(x.PlayerId) == null) || role.UnwarpablePlayers.ContainsKey(x.PlayerId))).ToList();

                    if (role.WarpPlayer1 == null)
                        role.WarpMenu1.Open(targets);
                    else if (role.WarpPlayer2 == null)
                        role.WarpMenu2.Open(targets);
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Warp);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.WarpPlayer1.PlayerId);
                        writer.Write(role.WarpPlayer2.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        Coroutines.Start(role.WarpPlayers());
                        role.LastWarped = DateTime.UtcNow;
                    }
                }

                return false;
            }

            return true;
        }
    }
}