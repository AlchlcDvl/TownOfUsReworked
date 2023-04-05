using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class StartMeetingPatch
    {
        public static void Prefix()
        {
            foreach (var cryo in Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
            {
                if (cryo.FreezeUsed)
                {
                    foreach (var player in cryo.DousedPlayers)
                    {
                        var player2 = Utils.PlayerById(player);

                        if (player2.Data.IsDead || player2.Data.Disconnected || player2.Is(RoleEnum.Pestilence))
                            continue;

                        Utils.RpcMurderPlayer(cryo.Player, player2);
                    }

                    cryo.DousedPlayers.Clear();
                    cryo.FreezeUsed = false;
                }
            }
        }
    }
}