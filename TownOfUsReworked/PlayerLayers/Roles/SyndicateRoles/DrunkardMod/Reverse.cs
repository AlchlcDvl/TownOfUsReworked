using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using System.Collections.Generic;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    public class Reverse
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class ConfuseFunctions
        {
            private static List<PlayerControl> Confused = new List<PlayerControl>();

            public static void ConfusedAll()
            {
                Confused.Clear();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsDead || player.Data.Disconnected || (player.Is(Faction.Syndicate) && CustomGameOptions.IntruderImmunity))
                        continue;
                    
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
