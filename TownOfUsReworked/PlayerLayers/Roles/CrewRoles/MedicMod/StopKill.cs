using Reactor.Utilities;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    public static class StopKill
    {
        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            var role = Role.GetRole<Medic>(Utils.PlayerById(medicId));

            if ((PlayerControl.LocalPlayer.PlayerId == playerId && (CustomGameOptions.NotificationShield == NotificationOptions.Shielded || CustomGameOptions.NotificationShield ==
                NotificationOptions.ShieldedAndMedic)) || (PlayerControl.LocalPlayer.PlayerId == medicId && (CustomGameOptions.NotificationShield == NotificationOptions.Medic ||
                CustomGameOptions.NotificationShield == NotificationOptions.ShieldedAndMedic)) || CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
            {
                Coroutines.Start(Utils.FlashCoroutine(role.Color));
            }

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role2 in Role.GetRoles(RoleEnum.Medic))
            {
                if (((Medic)role2).ShieldedPlayer.PlayerId == playerId)
                {
                    ((Medic)role2).ShieldedPlayer = null;
                    ((Medic)role2).ExShielded = player;
                    Utils.LogSomething(player.name + " Is Ex-Shielded");
                }
            }

            player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.MyRend().material.SetFloat("_Outline", 0f);
        }
    }
}