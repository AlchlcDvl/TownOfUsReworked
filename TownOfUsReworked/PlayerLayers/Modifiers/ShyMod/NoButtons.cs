using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Modifiers.ShyMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public static class NoButtons
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Shy))
                PlayerControl.LocalPlayer.RemainingEmergencies = 0;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public static class NoButtonsHost
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Shy))
                PlayerControl.LocalPlayer.RemainingEmergencies = 0;
        }
    }
}