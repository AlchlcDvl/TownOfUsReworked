using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Functions
{
    [HarmonyPatch]
    public static class Reverse
    {
        public static class ConfuseFunctions
        {
            private static readonly List<PlayerControl> Confused = new();

            public static void ConfuseAll()
            {
                Confused.Clear();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsDead || player.Data.Disconnected || (player.Is(Faction.Syndicate) && CustomGameOptions.SyndicateImmunity) || player.Is(RoleEnum.Drunkard) ||
                        player.Is(ModifierEnum.Drunk))
                    {
                        continue;
                    }

                    Confused.Add(player);
                }

                foreach (var player in Confused)
                {
                    if (player.CanMove && !MeetingHud.Instance && !(player.Data.IsDead || player.Data.Disconnected))
                        player.MyPhysics.Speed *= -1;
                }

                Utils.Flash(Colors.Drunkard, "You are confused!");
            }

            public static void UnconfuseAll()
            {
                foreach (var player in Confused)
                {
                    if (player.CanMove && !MeetingHud.Instance && !player.Data.Disconnected)
                        player.MyPhysics.Speed *= -1;
                }

                Confused.Clear();
                Utils.Flash(Colors.Drunkard, "You are no longer confused!");
            }
        }
    }
}