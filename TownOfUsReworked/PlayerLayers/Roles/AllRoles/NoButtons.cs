using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    public static class NoButtons
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
        public static class NoButton
        {
            public static void Postfix()
            {
                if ((!CustomGameOptions.MayorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) || (!CustomGameOptions.SwapperButton && PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                    || (!CustomGameOptions.ActorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Actor)) || (!CustomGameOptions.ExecutionerButton &&
                    PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) || (!CustomGameOptions.GuesserButton && PlayerControl.LocalPlayer.Is(RoleEnum.Guesser)) ||
                    (!CustomGameOptions.JesterButton && PlayerControl.LocalPlayer.Is(RoleEnum.Jester)))
                {
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
        public static class NoButtonHost
        {
            public static void Postfix()
            {
                if ((!CustomGameOptions.MayorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) || (!CustomGameOptions.SwapperButton && PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                    || (!CustomGameOptions.ActorButton && PlayerControl.LocalPlayer.Is(RoleEnum.Actor)) || (!CustomGameOptions.ExecutionerButton &&
                    PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) || (!CustomGameOptions.GuesserButton && PlayerControl.LocalPlayer.Is(RoleEnum.Guesser)) ||
                    (!CustomGameOptions.JesterButton && PlayerControl.LocalPlayer.Is(RoleEnum.Jester)))
                {
                    PlayerControl.LocalPlayer.RemainingEmergencies = 0;
                }
            }
        }
    }
}