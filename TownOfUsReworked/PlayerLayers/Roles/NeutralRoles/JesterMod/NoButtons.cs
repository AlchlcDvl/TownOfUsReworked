using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class NoButtons
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.JesterButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public class NoButtonsHost
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.JesterButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }
}