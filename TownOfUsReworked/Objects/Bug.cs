using System.Collections;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using HarmonyLib;
using Reactor.Utilities;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class Bug : Range
    {
        public Dictionary<byte, float> Players = new();

        public Bug(Vector2 position) : base(position, Colors.Operative, CustomGameOptions.BugRange, "Bug") => Coroutines.Start(Timer());

        public override IEnumerator Timer()
        {
            while (Transform != null)
            {
                yield return 0;
                Update();
            }
        }

        public override void Update()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead)
                    continue;

                if (Vector2.Distance(Transform.position, player.GetTruePosition()) < CustomGameOptions.BugRange)
                {
                    if (!Players.ContainsKey(player.PlayerId))
                        Players.Add(player.PlayerId, 0f);
                }
                else if (Players.ContainsKey(player.PlayerId))
                    Players.Remove(player.PlayerId);

                var entry = player;

                if (Players.ContainsKey(entry.PlayerId))
                {
                    Players[entry.PlayerId] += Time.deltaTime;

                    if (Players[entry.PlayerId] >= CustomGameOptions.MinAmountOfTimeInBug)
                    {
                        foreach (var t in Role.GetRoles<Operative>(RoleEnum.Operative))
                        {
                            if (t.Bugs.Contains(this))
                            {
                                var playerrole = Role.GetRole(Utils.PlayerById(entry.PlayerId)).RoleType;

                                if (!t.BuggedPlayers.Contains(playerrole) && entry != t.Player)
                                    t.BuggedPlayers.Add(playerrole);
                            }
                        }

                        foreach (var t in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
                        {
                            if (!t.IsOP)
                                continue;

                            if (t.Bugs.Contains(this))
                            {
                                var playerrole = Role.GetRole(Utils.PlayerById(entry.PlayerId)).RoleType;

                                if (!t.BuggedPlayers.Contains(playerrole) && entry != t.Player)
                                    t.BuggedPlayers.Add(playerrole);
                            }
                        }
                    }
                }
            }
        }

        public static void Clear(List<Bug> obj)
        {
            foreach (var t in obj)
                t.Destroy();
        }
    }
}