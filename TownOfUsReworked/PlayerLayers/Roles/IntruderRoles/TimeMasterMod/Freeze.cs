using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using System.Collections.Generic;
using Reactor.Utilities;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    public class Freeze
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysics_FixedUpdate
        {
            public static void FreezeAll()
            {
                var frozen = new List<PlayerControl>();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsDead || player.Data.Disconnected || (player.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) || (player.Is(RoleEnum.TimeMaster) &&
                        CustomGameOptions.TMImmunity) || (player.Is(Faction.Intruder) && CustomGameOptions.IntruderImmunity))
                        continue;
                    
                    frozen.Add(player);

                    if (player == PlayerControl.LocalPlayer)
                        Coroutines.Start(Utils.FlashCoroutine(Colors.TimeMaster));
                }

                foreach (var player in frozen)
                {
                    if (player.MyPhysics.myPlayer.CanMove && !MeetingHud.Instance && !(player.Data.IsDead || player.Data.Disconnected))
                    {
                        player.NetTransform.Halt();
                        player.moveable = false;
                    }
                }
            }

            public static void UnfreezeAll()
            {
                var frozen = new List<PlayerControl>();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsDead || player.Data.Disconnected || (player.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) || (player.Is(RoleEnum.TimeMaster) &&
                        CustomGameOptions.TMImmunity) || (player.Is(Faction.Intruder) && CustomGameOptions.IntruderImmunity))
                        continue;
                    
                    frozen.Add(player);

                    if (player == PlayerControl.LocalPlayer)
                        Coroutines.Start(Utils.FlashCoroutine(Colors.TimeMaster));
                }

                foreach (var player in frozen)
                {
                    if (player.MyPhysics.myPlayer.CanMove && !MeetingHud.Instance && player == player.MyPhysics.myPlayer && !player.MyPhysics.myPlayer.Data.Disconnected)
                        player.moveable = true;
                }
            }
        }
    }
}
