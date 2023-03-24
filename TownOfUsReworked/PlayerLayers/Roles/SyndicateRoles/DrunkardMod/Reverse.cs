using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
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

                Coroutines.Start(Utils.FlashCoroutine(Colors.Drunkard));
            }

            public static void UnconfuseAll()
            {
                foreach (var player in Confused)
                {
                    if (player.CanMove && !MeetingHud.Instance && !player.Data.Disconnected)
                        player.MyPhysics.Speed *= -1;
                }

                Confused.Clear();
                Coroutines.Start(Utils.FlashCoroutine(Colors.Drunkard));
            }
        }
    }
}
