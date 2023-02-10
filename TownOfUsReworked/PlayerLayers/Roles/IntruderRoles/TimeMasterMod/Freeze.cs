using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using System.Collections.Generic;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    public class Freeze
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class FreezeFunctions
        {
            private static List<PlayerControl> Frozen = new List<PlayerControl>();

            public static void FreezeAll()
            {
                Frozen.Clear();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsDead || player.Data.Disconnected || (player.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) || (player.Is(RoleEnum.TimeMaster) &&
                        CustomGameOptions.TMImmunity) || (player.Is(Faction.Intruder) && CustomGameOptions.IntruderImmunity))
                        continue;
                    
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

                Coroutines.Start(Utils.FlashCoroutine(Colors.TimeMaster));
            }

            public static void UnfreezeAll()
            {
                foreach (var player in Frozen)
                {
                    if (player.CanMove && !MeetingHud.Instance && !player.Data.Disconnected)
                        player.moveable = true;
                }

                Frozen.Clear();
                Coroutines.Start(Utils.FlashCoroutine(Colors.TimeMaster));
            }
        }
    }
}
