using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    public static class StopKill
    {
        public static void BreakShield(byte retId, byte playerId, bool flag)
        {
            if ((PlayerControl.LocalPlayer.PlayerId == playerId && (CustomGameOptions.NotificationShield == NotificationOptions.Shielded || CustomGameOptions.NotificationShield ==
                NotificationOptions.ShieldedAndMedic)) || (PlayerControl.LocalPlayer.PlayerId == retId && (CustomGameOptions.NotificationShield == NotificationOptions.Medic ||
                CustomGameOptions.NotificationShield == NotificationOptions.ShieldedAndMedic)) || CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
            {
                Utils.Flash(Colors.Medic, "Someone was attacked!");
            }

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role2 in Role.GetRoles(RoleEnum.Retributionist))
            {
                if (((Retributionist)role2).RevivedRole?.RoleType != RoleEnum.Medic)
                    continue;

                if (((Retributionist)role2).ShieldedPlayer.PlayerId == playerId)
                {
                    ((Retributionist)role2).ShieldedPlayer = null;
                    ((Retributionist)role2).ExShielded = player;
                    Utils.LogSomething(player.name + " Is Ex-Shielded");
                }
            }

            player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.MyRend().material.SetFloat("_Outline", 0f);
        }
    }
}