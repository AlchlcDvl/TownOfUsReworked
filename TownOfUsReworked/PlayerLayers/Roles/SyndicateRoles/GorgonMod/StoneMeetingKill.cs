using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class StoneMeetingKill
    {
        public static void Prefix()
        {
            foreach (var gorg in Role.GetRoles(RoleEnum.Gorgon))
            {
                var gorgon = (Gorgon)gorg;

                foreach (var id in gorgon.Gazed)
                {
                    var stoned = Utils.PlayerById(id);

                    if (stoned == null || stoned.Data?.Disconnected != false || stoned.Data.IsDead || stoned.Is(RoleEnum.Pestilence))
                        continue;

                    Utils.RpcMurderPlayer(gorg.Player, stoned, false);
                    stoned.moveable = true;
                }

                gorgon.Gazed.Clear();
            }
        }
    }
}