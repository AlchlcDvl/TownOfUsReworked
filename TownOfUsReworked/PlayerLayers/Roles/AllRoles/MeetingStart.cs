using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            EndGame.Reset();
            PlayerControl.LocalPlayer.RegenTask();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                var pb = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);

                foreach (var player in pb.InfectedPlayers)
                {
                    var player2 = Utils.PlayerById(player);

                    if (player2 == null || player2.Data == null)
                        continue;

                    if (player2.Data.IsDead || player2.Data.Disconnected)
                        pb.InfectedPlayers.Remove(player);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac))
            {
                var cryo = Role.GetRole<Cryomaniac>(PlayerControl.LocalPlayer);

                foreach (var player in cryo.DousedPlayers)
                {
                    var player2 = Utils.PlayerById(player);

                    if (player2 == null || player2.Data == null)
                        continue;

                    if (player2.Data.IsDead || player2.Data.Disconnected)
                        cryo.DousedPlayers.Remove(player);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                var arso = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

                foreach (var player in arso.DousedPlayers)
                {
                    var player2 = Utils.PlayerById(player);

                    if (player2 == null || player2.Data == null)
                        continue;

                    if (player2.Data.IsDead || player2.Data.Disconnected)
                        arso.DousedPlayers.Remove(player);
                }
            }
        }
    }
}
