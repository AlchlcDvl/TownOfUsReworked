using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    public class Freeze
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysics_FixedUpdate
        {
            public static void FreezeAll()
            {
                var tm = Role.GetRoleValue<TimeMaster>(RoleEnum.TimeMaster);
                var frozen = new List<PlayerControl>();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data.IsDead | player.Data.Disconnected | (player.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) |
                        player.Is(Faction.Intruders)))
                        frozen.Add(player);
                }

                foreach (var player in frozen)
                {
                    if (tm.Frozen)
                    {
                        if (player.MyPhysics.myPlayer.CanMove && !MeetingHud.Instance && !(player.Data.IsDead | player.Data.Disconnected))
                        {
                            if (player.MyPhysics.AmOwner)
                                player.MyPhysics.body.velocity *= 0;
                        }
                    }
                }
            }

            public static void UnfreezeAll()
            {
                var tm = Role.GetRoleValue<TimeMaster>(RoleEnum.TimeMaster);
                var frozen = new List<PlayerControl>();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data.IsDead | player.Data.Disconnected | (player.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) |
                        player.Is(Faction.Intruders)))
                        frozen.Add(player);
                }

                foreach (var player in frozen)
                {
                    if (tm.Frozen)
                    {
                        if (player.MyPhysics.myPlayer.CanMove && !MeetingHud.Instance && player == player.MyPhysics.myPlayer &&
                            !(player.MyPhysics.myPlayer.Data.IsDead | player.MyPhysics.myPlayer.Data.Disconnected))
                        {
                            if (player.MyPhysics.AmOwner)
                                player.MyPhysics.body.velocity = new Vector2(PlayerControl.GameOptions.PlayerSpeedMod, 0f);
                        }
                    }
                }
            }
        }
    }
}
