using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RecordRewinds
    {
        public static void Postfix()
        {
            if (RecordRewind.rewinding)
                RecordRewind.Rewind();
            else
                RecordRewind.Record();

            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var TimeLord = (TimeLord)role;

                if ((DateTime.UtcNow - TimeLord.StartRewind).TotalMilliseconds > CustomGameOptions.RewindDuration * 1000f && TimeLord.FinishRewind < TimeLord.StartRewind)
                    StartStop.StopRewind(TimeLord);
            }
        }
    }
}