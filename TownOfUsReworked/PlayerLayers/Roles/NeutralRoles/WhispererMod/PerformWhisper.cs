using HarmonyLib;
using Hazel;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WhispererMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformWhisper
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Whisperer))
                return true;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            if (__instance == role.WhisperButton)
            {
                if (role.WhisperTimer() != 0f)
                    return false;

                role.LastWhispered = DateTime.UtcNow;
                role.Whisper();
                return false;
            }

            return true;
        }
    }
}