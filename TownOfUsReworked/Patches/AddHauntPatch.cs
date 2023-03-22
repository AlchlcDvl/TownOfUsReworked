using HarmonyLib;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class AddHauntPatch
    {
        public static List<PlayerControl> AssassinatedPlayers = new List<PlayerControl>();

        public static void Prefix(ExileController __instance)
        {
            foreach (var player in AssassinatedPlayers)
                player.Exiled();

            AssassinatedPlayers.Clear();

            foreach (var role in Role.GetRoles(RoleEnum.Ghoul))
            {
                var ghoul = (Ghoul)role;

                if (ghoul.Caught)
                {
                    ghoul.MarkedPlayer = null;
                    continue;
                }

                if (ghoul.MarkedPlayer != null && !(ghoul.MarkedPlayer.Data.IsDead || ghoul.MarkedPlayer.Data.Disconnected))
                    ghoul.MarkedPlayer.Exiled();
            }
        }
    }
}