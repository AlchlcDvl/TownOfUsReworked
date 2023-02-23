using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    public class Stone
    {
        private static List<(PlayerControl, float, bool)> NewGazed = new List<(PlayerControl, float, bool)>();

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysics_FixedUpdate
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Gorgon))
                {
                    var gorgon = (Gorgon)role;
                    NewGazed.Clear();

                    foreach (var tuple in gorgon.Gazed)
                    {
                        var player = tuple.Item1;
                        var stoned = false;

                        if (player.Data.IsDead || player.Data.Disconnected)
                        {
                            player.moveable = true;
                            continue;
                        }

                        var time = tuple.Item2;

                        if (__instance.myPlayer == player && !(player.Data.IsDead || player.Data.Disconnected))
                        {
                            if (time < CustomGameOptions.GazeTime + CustomGameOptions.GazeDelay)
                                time += Time.deltaTime;

                            if (time > CustomGameOptions.GazeDelay && (time - CustomGameOptions.GazeDelay) < CustomGameOptions.GazeTime)
                                __instance.body.velocity = __instance.body.velocity / (time - CustomGameOptions.GazeDelay);

                            if (time >= CustomGameOptions.GazeTime + CustomGameOptions.GazeDelay)
                            {
                                player.NetTransform.Halt();
                                player.moveable = false;
                                __instance.body.velocity *= 0;
                                stoned = true;
                                player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);
                                player.nameText().text += "\nFROZEN";
                            }
                        }

                        if (!(player.Data.IsDead || player.Data.Disconnected))
                            NewGazed.Add((player, time, stoned));
                    }

                    gorgon.Gazed.Clear();
                    gorgon.Gazed = NewGazed;
                }
            }
        }
    }
}

//69 Lines haha nice