using System.Collections;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using System.Linq;
using HarmonyLib;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class Bug
    {
        public Dictionary<byte, float> Players = new();
        public Transform Transform;

        public IEnumerator BugTimer()
        {
            while (Transform != null)
            {
                yield return 0;
                Update();
            }
        }

        public void Update()
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead)
                    continue;

                if (Vector2.Distance(Transform.position, player.GetTruePosition()) < CustomGameOptions.BugRange + 0.05f)
                {
                    if (!Players.ContainsKey(player.PlayerId))
                        Players.Add(player.PlayerId, 0f);
                }
                else
                {
                    if (Players.ContainsKey(player.PlayerId))
                        Players.Remove(player.PlayerId);
                }

                var entry = player;

                if (Players.ContainsKey(entry.PlayerId))
                {
                    Players[entry.PlayerId] += Time.deltaTime;

                    if (Players[entry.PlayerId] > CustomGameOptions.MinAmountOfTimeInBug)
                    {
                        foreach (var t in Role.GetRoles(RoleEnum.Operative).Cast<Operative>())
                        {
                            if (t.Bugs.Contains(this))
                            {
                                var playerrole = Role.GetRole(Utils.PlayerById(entry.PlayerId)).Type;

                                if (!t.BuggedPlayers.Contains(playerrole) && entry != t.Player)
                                    t.BuggedPlayers.Add(playerrole);
                            }
                        }

                        foreach (var t in Role.GetRoles(RoleEnum.Retributionist).Cast<Retributionist>())
                        {
                            if (t.RevivedRole?.Type != RoleEnum.Operative)
                                continue;

                            if (t.Bugs.Contains(this))
                            {
                                var playerrole = Role.GetRole(Utils.PlayerById(entry.PlayerId)).Type;

                                if (!t.BuggedPlayers.Contains(playerrole) && entry != t.Player)
                                    t.BuggedPlayers.Add(playerrole);
                            }
                        }
                    }
                }
            }
        }
    }
}