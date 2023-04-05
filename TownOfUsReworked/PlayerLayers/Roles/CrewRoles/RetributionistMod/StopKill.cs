using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch]
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

            foreach (var role2 in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (role2.RevivedRole?.RoleType != RoleEnum.Medic)
                    continue;

                if (role2.ShieldedPlayer.PlayerId == playerId)
                {
                    role2.ShieldedPlayer = null;
                    role2.ExShielded = player;
                    Utils.LogSomething(player.name + " Is Ex-Shielded");
                }
            }

            player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.MyRend().material.SetFloat("_Outline", 0f);
        }
    }
}