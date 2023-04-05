using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    [HarmonyPatch]
    public static class StopKill
    {
        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            var role = Role.GetRole<Medic>(Utils.PlayerById(medicId));

            if ((PlayerControl.LocalPlayer.PlayerId == playerId && (CustomGameOptions.NotificationShield == NotificationOptions.Shielded || CustomGameOptions.NotificationShield ==
                NotificationOptions.ShieldedAndMedic)) || (PlayerControl.LocalPlayer.PlayerId == medicId && (CustomGameOptions.NotificationShield == NotificationOptions.Medic ||
                CustomGameOptions.NotificationShield == NotificationOptions.ShieldedAndMedic)) || CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
            {
                Utils.Flash(role.Color, "Someone was attacked but they were nursed back to health!");
            }

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role2 in Role.GetRoles<Medic>(RoleEnum.Medic))
            {
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