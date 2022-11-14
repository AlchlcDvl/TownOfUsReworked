using HarmonyLib;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class NoButtons
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.SwapperButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public class NoButtonsHost
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.SwapperButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }
}