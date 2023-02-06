using HarmonyLib;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MayorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class NoButtons
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.MayorButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public class NoButtonsHost
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.MayorButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }
}