using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ActorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class NoButtons
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.ActorButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Actor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public class NoButtonsHost
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.ActorButton)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Actor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }
}