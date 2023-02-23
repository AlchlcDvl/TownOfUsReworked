using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    class NoButtons
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
        public class NoButton
        {
            public static void Postfix()
            {
                if (!CustomGameOptions.MayorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.SwapperButton && PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.ActorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Actor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.ExecutionerButton && PlayerControl.LocalPlayer.Is(RoleEnum.Executioner))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.GuesserButton && PlayerControl.LocalPlayer.Is(RoleEnum.Guesser))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.JesterButton && PlayerControl.LocalPlayer.Is(RoleEnum.Jester))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
        public class NoButtonHost
        {
            public static void Postfix()
            {
                if (!CustomGameOptions.MayorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Mayor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.SwapperButton && PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.ActorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Actor))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.ExecutionerButton && PlayerControl.LocalPlayer.Is(RoleEnum.Executioner))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.GuesserButton && PlayerControl.LocalPlayer.Is(RoleEnum.Guesser))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                else if (!CustomGameOptions.JesterButton && PlayerControl.LocalPlayer.Is(RoleEnum.Jester))
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
            }
        }
    }
}