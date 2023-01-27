using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted, true))
                return false;

            var role = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (__instance != role.KillButton)
                return false;

            if (Utils.IsTooFar(PlayerControl.LocalPlayer, role.ClosestPlayer))
                return false;

            if (role.KillTimer() != 0f && __instance == role.KillButton)
                return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, null, true);

            if (interact[3] == true && interact[0] == true)
                return false;
            else if (interact[1] == true)
                role.LastKilled = role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2] == true)
                role.LastKilled = role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
            else if (interact[3] == true)
                return false;

            return false;
        }
    }
}