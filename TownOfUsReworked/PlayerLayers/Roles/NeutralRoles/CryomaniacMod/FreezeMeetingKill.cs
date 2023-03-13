using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class StartMeetingPatch
    {
        public static void Prefix()
        {
            foreach (var cryo in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                var role = (Cryomaniac)cryo;

                if (role.FreezeUsed)
                {
                    foreach (var player in role.DousedPlayers)
                    {
                        var player2 = Utils.PlayerById(player);

                        if (player2.Data.IsDead || player2.Data.Disconnected || player2.Is(RoleEnum.Pestilence))
                            continue;

                        Utils.RpcMurderPlayer(role.Player, player2, false);
                    }

                    role.DousedPlayers.Clear();
                    role.FreezeUsed = false;
                }
            }
        }
    }
}