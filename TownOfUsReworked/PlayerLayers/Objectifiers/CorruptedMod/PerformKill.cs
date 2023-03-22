using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Corrupted))
                return true;

            var objectifier = Objectifier.GetObjectifier<Corrupted>(PlayerControl.LocalPlayer);

            if (__instance == objectifier.KillButton)
            {
                if (objectifier.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(objectifier.Player, objectifier.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(objectifier.Player, objectifier.ClosestPlayer, true);

                if (interact[3] == true || interact[0] == true)
                    objectifier.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    objectifier.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    objectifier.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
                
                return false;
            }

            return true;
        }
    }
}