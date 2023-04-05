using HarmonyLib;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    public static class AddHauntPatch
    {
        public readonly static List<PlayerControl> AssassinatedPlayers = new();

        public static void Prefix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var player in AssassinatedPlayers)
                player.Exiled();

            AssassinatedPlayers.Clear();

            foreach (var ghoul in Role.GetRoles<Ghoul>(RoleEnum.Ghoul))
            {
                if (ghoul.Caught)
                    ghoul.MarkedPlayer = null;
                else if (ghoul.MarkedPlayer != null && !(ghoul.MarkedPlayer.Data.IsDead || ghoul.MarkedPlayer.Data.Disconnected))
                    ghoul.MarkedPlayer.Exiled();
            }
        }
    }
}