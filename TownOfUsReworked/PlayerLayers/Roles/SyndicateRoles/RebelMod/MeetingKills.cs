using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.RebelMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Rebel))
            {
               var rebel = (Rebel)role;

                if (rebel.FormerRole == null || rebel.FormerRole?.RoleType == RoleEnum.Anarchist || !rebel.WasSidekick)
                    continue;

                if (rebel.FormerRole.RoleType == RoleEnum.Poisoner)
                {
                    if (rebel.Player != rebel.PoisonedPlayer && rebel.PoisonedPlayer != null)
                    {
                        if (!rebel.PoisonedPlayer.Data.IsDead && !rebel.PoisonedPlayer.Is(RoleEnum.Pestilence))
                            Utils.MurderPlayer(rebel.Player, rebel.PoisonedPlayer);
                    }
                }
                else if (rebel.FormerRole.RoleType == RoleEnum.Gorgon)
                {
                    foreach (var tuple in rebel.Gazed)
                    {
                        var stoned = tuple.Item3;
                        var time = tuple.Item2;
                        var target = tuple.Item1;

                        if (stoned || time >= CustomGameOptions.GazeTime + CustomGameOptions.GazeDelay && !(target.Data.IsDead || target.Data.Disconnected))
                            Utils.RpcMurderPlayer(rebel.Player, target, false);
                        
                        target.moveable = true;
                    }

                    rebel.Gazed.Clear();
                }
            }
        }
    }
}