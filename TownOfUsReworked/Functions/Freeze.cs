using HarmonyLib;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Functions
{
    [HarmonyPatch]
    public static class Freeze
    {
        public static class FreezeFunctions
        {
            private readonly static List<PlayerControl> Frozen = new();

            public static void FreezeAll()
            {
                Frozen.Clear();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsDead || player.Data.Disconnected || (player.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) || (player.Is(RoleEnum.TimeMaster) &&
                        CustomGameOptions.TMImmunity) || (player.Is(Faction.Intruder) && CustomGameOptions.IntruderImmunity))
                    {
                        continue;
                    }

                    Frozen.Add(player);
                }

                foreach (var player in Frozen)
                {
                    if (player.CanMove && !MeetingHud.Instance && !(player.Data.IsDead || player.Data.Disconnected))
                    {
                        player.NetTransform.Halt();
                        player.moveable = false;
                    }
                }

                Utils.Flash(Colors.TimeMaster, "Time is frozen!");
            }

            public static void UnfreezeAll()
            {
                foreach (var player in Frozen)
                {
                    if (player.CanMove && !MeetingHud.Instance && !player.Data.Disconnected)
                        player.moveable = true;
                }

                Frozen.Clear();
                Utils.Flash(Colors.TimeMaster, "Time is no longer frozen!");
            }
        }
    }
}