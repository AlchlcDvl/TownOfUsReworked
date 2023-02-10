using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class StoneMeetingKill
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
                return;

            foreach (var gorg in Role.GetRoles(RoleEnum.Gorgon))
            {
                var gorgon = (Gorgon)gorg;

                foreach (var tuple in gorgon.Gazed)
                {
                    var stoned = tuple.Item3;
                    var time = tuple.Item2;
                    var target = tuple.Item1;

                    if (stoned || time >= CustomGameOptions.GazeTime + CustomGameOptions.GazeDelay && !(target.Data.IsDead || target.Data.Disconnected))
                        Utils.RpcMurderPlayer(gorgon.Player, target);
                    
                    target.moveable = true;
                }

                gorgon.Gazed.Clear();
            }
        }
    }
}