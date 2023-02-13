using HarmonyLib;
using Reactor.Utilities;
using System.Collections;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    public class Bug
    {
        public Dictionary<byte, float> players = new Dictionary<byte, float>();
        public Transform transform;

        public IEnumerator BugTimer()
        {
            while (transform != null)
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

                if (Vector2.Distance(transform.position, player.GetTruePosition()) < CustomGameOptions.BugRange + 0.05f)
                {
                    if (!players.ContainsKey(player.PlayerId))
                        players.Add(player.PlayerId, 0f);
                }
                else
                {
                    if (players.ContainsKey(player.PlayerId))
                        players.Remove(player.PlayerId);
                }

                var entry = player;

                if (players.ContainsKey(entry.PlayerId))
                {
                    players[entry.PlayerId] += Time.deltaTime;

                    if (players[entry.PlayerId] > CustomGameOptions.MinAmountOfTimeInBug)
                    {
                        foreach (Retributionist t in Role.GetRoles(RoleEnum.Retributionist))
                        {
                            if (t.RevivedRole?.RoleType != RoleEnum.Operative)
                                continue;

                            RoleEnum playerrole = Role.GetRole(Utils.PlayerById(entry.PlayerId)).RoleType;

                            if (!t.BuggedPlayers.Contains(playerrole) && entry != t.Player)
                                t.BuggedPlayers.Add(playerrole);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch]
    public static class BugExtentions2
    {
        public static void ClearBugs(this List<Bug> obj)
        {
            foreach (Bug t in obj)
            {
                Object.Destroy(t.transform.gameObject);
                Coroutines.Stop(t.BugTimer());
            }

            obj.Clear();
        }

        public static Bug CreateBug(this Vector3 location)
        {
            var BugPref = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BugPref.name = "Bug";
            BugPref.transform.localScale = new Vector3(CustomGameOptions.BugRange, CustomGameOptions.BugRange, CustomGameOptions.BugRange);
            GameObject.Destroy(BugPref.GetComponent<SphereCollider>());
            BugPref.GetComponent<MeshRenderer>().material = Retributionist.BugMaterial;
            BugPref.transform.position = location;
            var BugScript = new Bug();
            BugScript.transform = BugPref.transform;
            Coroutines.Start(BugScript.BugTimer());
            return BugScript;
        }
    }
}