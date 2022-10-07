using HarmonyLib;

namespace TownOfUs.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class NoButtons
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.ExecutionerButton)
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public class NoButtonsHost
    {
        public static void Postfix()
        {
            if (!CustomGameOptions.ExecutionerButton) 
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
        }
    }
}